using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Timers;
using Stein.Models;
using Stein.Services;

namespace Stein.Routines
{
    /// <summary>
    /// Provides file watching and HTTP server capabilities for a project.
    /// </summary>
    public sealed class ServeRoutine : Routine
    {
        /// <summary>The port to run the server on.</summary>
        private string ServerPort { get; }

        /// <summary>
        /// The server addresses, defaults to localhost + ServerPort or 
        /// http://localhost:8000 by default.
        /// </summary>
        private string[] ServerPrefixes { get; } = { "http://localhost:" };

        /// <summary>
        /// Used to store files that have recently emitted events. When an event is detected,
        /// the event will be ignored if the file exists in this list. This is necessary because
        /// .NET emits multiple events from a single logical action, and that would cause multiple 
        /// project rebuilds for a single change.
        /// </summary>
        private List<string> ServerCache { get; } = new();

        public ServeRoutine(string port = "8000")
        {
            ServerPort = port;
            ServerPrefixes[0] += $"{port}/";
        }

        /// <summary>
        /// Watch a project for changes to the resources directory, trigger BuildRoutine.Execute() when
        /// a change is detected.
        /// </summary>
        public override void Execute()
        {
            HttpListener listener = new HttpListener();
            foreach (string prefix in ServerPrefixes) listener.Prefixes.Add(prefix);

            FileSystemWatcher watcher = new(PathService.ResourcesPath);
            watcher.IncludeSubdirectories = true;

            watcher.NotifyFilter =
                NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastWrite
                | NotifyFilters.Size;

            watcher.Changed += HandleEvent;
            watcher.Deleted += HandleEvent;
            watcher.Renamed += HandleEvent;
            watcher.Error += HandleError;

            watcher.EnableRaisingEvents = true;

            listener.Start();

            string projectName = Path.GetFileName(Directory.GetCurrentDirectory());
            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize($"Serving {projectName} ", ConsoleColor.White, false);
            StringService.Colorize($"on http://localhost:{ServerPort}", ConsoleColor.White, true);

            while (true)
            {
                HttpListenerContext context = listener.GetContext();

                byte[] buffer = Array.Empty<byte>();

                string requestedFileName = Path.GetFileName(context.Request.RawUrl);

                string requestedFile = !Path.HasExtension(requestedFileName)
                    ? Path.Join(context.Request.RawUrl, "index.html")
                    : context.Request.RawUrl;

                StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
                StringService.Colorize($"{context.Request.RawUrl} ", ConsoleColor.White, false);

                if (File.Exists(Path.Join(PathService.SitePath, requestedFile)))
                {
                    StringService.Colorize("200", ConsoleColor.Green, true);
                    buffer = File.ReadAllBytes(Path.Join(PathService.SitePath, requestedFile));
                    context.Response.StatusCode = 200;

                    string extension = Path.GetExtension(requestedFile);

                    context.Response.ContentType = extension switch
                    {
                        ".png" => "image/png",
                        ".bmp" => "image/bmp",
                        ".gif" => "image/gif",
                        ".jpeg" => "image/jpeg",
                        ".jpg" => "image/jpeg",
                        ".svg" => "image/svg+xml",
                        ".tif" => "image/tiff",
                        ".tiff" => "image/tiff",
                        ".webp" => "image/webp",
                        _ => context.Response.ContentType
                    };

                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer);
                }
                else
                {
                    StringService.Colorize("404", ConsoleColor.Red, true);
                    context.Response.StatusCode = 404;
                }

                context.Response.OutputStream.Close();
            }
        }

        private void HandleEvent(object sender, FileSystemEventArgs e)
        {
            if (ServerCache.Contains(e.FullPath)) return;
            ServerCache.Add(e.FullPath);

            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                FullRebuild();
            }

            Timer timer = new Timer(100) { AutoReset = false };

            timer.Elapsed += (timerElapsedSender, timerElapsedArgs) =>
            {
                lock (ServerCache)
                {
                    ServerCache.Remove(e.FullPath);
                }
            };

            timer.Start();
        }

        private void FullRebuild()
        {
            BuildRoutine build = new();
            build.Execute();
        }

        private void HandleError(object sender, ErrorEventArgs e)
        {
            MessageService.Log(new Message($"Server restart is required: ({e.GetException().Message})", Message.InfoType.Error));
            MessageService.Print(true);
        }
    }
}