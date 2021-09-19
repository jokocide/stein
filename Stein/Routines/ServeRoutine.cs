using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Timers;
using Stein.Interfaces;
using Stein.Models;
using Stein.Services;

namespace Stein.Routines
{
    public sealed class ServeRoutine : IExecutable
    {
        private string Port { get; }

        private string Address { get; } = "http://localhost:";

        /// <summary>
        /// Temporarily store the name of files that have emitted an event 
        /// here, necessary to avoid multiple event emits from a single change.
        /// </summary>
        private List<string> Cache { get; } = new();

        public ServeRoutine(string port = "8000")
        {
            Port = port;
            Address += $"{port}/";
        }

        public void Execute()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(Address);

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
            StringService.Colorize($"on http://localhost:{Port}", ConsoleColor.White, true);

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
            if (Cache.Contains(e.FullPath)) return;
            Cache.Add(e.FullPath);

            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                FullRebuild();
            }

            Timer timer = new Timer(100) { AutoReset = false };

            timer.Elapsed += (timerElapsedSender, timerElapsedArgs) =>
            {
                lock (Cache)
                {
                    Cache.Remove(e.FullPath);
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