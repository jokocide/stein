using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;
using Dagger.Models;
using Dagger.Services;

namespace Dagger.Routines
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
            string resources = Path.Join(Directory.GetCurrentDirectory(), "resources");
            string site = Path.Join(Directory.GetCurrentDirectory(), "site");
            
            // Server initialization.
            HttpListener listener = new HttpListener();
            foreach (string prefix in ServerPrefixes) listener.Prefixes.Add(prefix);

            // Watcher initialization.
            FileSystemWatcher watcher = new(resources);
            
            watcher.NotifyFilter = NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastWrite
                | NotifyFilters.Size;
            
            watcher.Changed += OnUpdate;
            watcher.Created += OnUpdate;
            watcher.Deleted += OnUpdate;
            watcher.Renamed += OnUpdate;
            watcher.Error += OnError;
            
            // Default filter will watch all files.
            watcher.Filters.Add("*.md");
            watcher.Filters.Add("*.hbs");
            watcher.Filters.Add("*.html");
            watcher.Filters.Add("*.js");
            watcher.Filters.Add("*.css");
            watcher.Filters.Add("*.scss");
            watcher.Filters.Add("*.sass");

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            
            listener.Start();
            
            StringService.Colorize(ConsoleColor.Cyan, "Serving project on ", false);
            StringService.Colorize(ConsoleColor.DarkGray, $"http://localhost:{ServerPort}");
            StringService.Colorize(ConsoleColor.DarkGray, "Logging requests:");

            while (true)
            {
                // Wait for a request.
                HttpListenerContext context = listener.GetContext(); // WARNING: This is not async.
                
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                
                // Log the requested file.
                StringService.Colorize(ConsoleColor.DarkGray, $"({DateTime.Now:T})", false);
                StringService.Colorize(ConsoleColor.Cyan, $" {request.RawUrl}");

                string requestedFileName = Path.GetFileName(request.RawUrl);
                string requestedFile = !Path.HasExtension(requestedFileName) ? Path.Join(request.RawUrl, "index.html") : request.RawUrl;

                // Return a 404 for files that don't exist.
                byte[] buffer;
                
                if (!File.Exists(Path.Join(site, requestedFile)))
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
                
                buffer = File.ReadAllBytes(Path.Join(site, requestedFile));
                
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer);
                output.Close();
            }
        }
        
        // Todo: Document this!
        private void OnUpdate(object sender, FileSystemEventArgs e)
        {
            if (ServerCache.Contains(e.FullPath)) return;
            ServerCache.Add(e.FullPath);
            
            BuildRoutine build = new();
            StringService.Colorize(ConsoleColor.DarkGray, $"({DateTime.Now:T}) ", false);
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