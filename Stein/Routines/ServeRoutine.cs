using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
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
        /// <summary>
        /// The port to run the server on.
        /// </summary>
        private string ServerPort { get; }
        
        /// <summary>
        /// The server address(es).
        /// </summary>
        private string[] ServerPrefixes { get; } = { "http://localhost:" };
        
        /// <summary>
        /// .NET will emit multiple events when a file is accessed in certain cases, so the file paths are stored here
        /// and removed shortly after to prevent duplicate rebuilds.
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
            
            watcher.Changed += Build;
            watcher.Created += Build;
            watcher.Deleted += Build;
            watcher.Renamed += Build;
            watcher.Error += OnError;
            
            watcher.EnableRaisingEvents = true;
            
            listener.Start();
            
            StringService.Colorize("Serving project on ", ConsoleColor.White, false);
            StringService.Colorize($"http://localhost:{ServerPort}", ConsoleColor.Gray, true);

            while (true)
            {
                // Block while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                
                byte[] buffer = Array.Empty<byte>();

                string requestedFileName = Path.GetFileName(context.Request.RawUrl);

                string requestedFile = !Path.HasExtension(requestedFileName)
                    ? Path.Join(context.Request.RawUrl, "index.html")
                    : context.Request.RawUrl;
                
                if (File.Exists(Path.Join(PathService.SitePath, requestedFile)))
                {
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
                    StringService.Colorize($" {context.Request.RawUrl}", ConsoleColor.Red, false);
                    StringService.Colorize(" (404)", ConsoleColor.Gray, true);
                    context.Response.StatusCode = 404;
                }

                context.Response.OutputStream.Close();
            }
        }
        
        // Todo: Document this!
        private void Build(object sender, FileSystemEventArgs e)
        {
            if (ServerCache.Contains(e.FullPath)) return;
            ServerCache.Add(e.FullPath);
            
            BuildRoutine build = new();
            build.Execute();
            
            Timer timer = new Timer(100) {AutoReset = false};
            
            timer.Elapsed += (timerElapsedSender, timerElapsedArgs) =>
            {
                lock (ServerCache)
                {
                    ServerCache.Remove(e.FullPath);
                }
            };
            
            timer.Start();
        }
        
        // Todo: Document this!
        private void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        // Todo: Document this!
        private void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}