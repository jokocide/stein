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
            // Server initialization.
            HttpListener listener = new HttpListener();
            foreach (string prefix in ServerPrefixes) listener.Prefixes.Add(prefix);

            // Watcher initialization.
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
            
            // Default filter will watch all files.
            // watcher.Filters.Add("*.md");
            // watcher.Filters.Add("*.hbs");
            // watcher.Filters.Add("*.html");
            // watcher.Filters.Add("*.js");
            // watcher.Filters.Add("*.css");
            // watcher.Filters.Add("*.scss");
            // watcher.Filters.Add("*.sass");

            // Watcher is enabled with this property.
            watcher.EnableRaisingEvents = true;
            
            listener.Start();
            
            StringService.Colorize("Serving project on ", ConsoleColor.White, false);
            StringService.Colorize($"http://localhost:{ServerPort}", ConsoleColor.Gray, true);

            while (true)
            {
                // Wait for a request.
                HttpListenerContext context = listener.GetContext(); // WARNING: This is not async.
                
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                
                // Log the requested file.
                StringService.Colorize($"({DateTime.Now:T})", ConsoleColor.Gray, false);
                StringService.Colorize($" {request.RawUrl}", ConsoleColor.White, true);

                string requestedFileName = Path.GetFileName(request.RawUrl);
                string requestedFile = !Path.HasExtension(requestedFileName) ? Path.Join(request.RawUrl, "index.html") : request.RawUrl;

                // Return a 404 for files that don't exist.
                byte[] buffer;
                
                if (!File.Exists(Path.Join(PathService.SitePath, requestedFile)))
                {
                    response.StatusCode = 404;
                    const string responseString = "<HTML><BODY>404</BODY></HTML>";
                    buffer = Encoding.UTF8.GetBytes(responseString);
                }

                // Set a content type.
                string extension = Path.GetExtension(requestedFile);
                response.ContentType = extension switch
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
                    _ => response.ContentType
                };
                
                // Todo: try/catch for files removed during project edit.
                buffer = File.ReadAllBytes(Path.Join(PathService.SitePath, requestedFile));
                
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer);
                output.Close();
            }
        }
        
        // Todo: Document this!
        private void Build(object sender, FileSystemEventArgs e)
        {
            if (ServerCache.Contains(e.FullPath)) return;
            ServerCache.Add(e.FullPath);
            
            BuildRoutine build = new();
            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
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