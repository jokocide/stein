using System;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;
using System.Collections.Generic;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Watch a Dagger project for changes to the resources directory, and trigger a Build routine as a result.
    /// </summary>
    public class ServeRoutine : Routine
    {
        private string ServerPort { get; }
        private bool ServerIsActive { get; set; } = true;
        private string[] ServerPrefixes { get; } = { "http://localhost:" };
        private BuildRoutine Build { get; } = new();
        private List<string> ChangedFiles { get; } = new();

        public ServeRoutine(string port = "8000")
        {
            ServerPort = port;
            ServerPrefixes[0] += $"{port}/";
        }

        public override void Execute()
        {
            string resources = Path.Join(Directory.GetCurrentDirectory(), "resources");
            string site = Path.Join(Directory.GetCurrentDirectory(), "site");
            
            // Server initialization.
            HttpListener listener = new HttpListener();
            byte[] buffer = Array.Empty<byte>();
            bool requestedFileExists = true;

            foreach (string prefix in ServerPrefixes)
                listener.Prefixes.Add(prefix);
            
            FileSystemWatcher watcher = new FileSystemWatcher(resources)
            {
                NotifyFilter = NotifyFilters.Attributes
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.FileName
                               | NotifyFilters.LastWrite
            };
            
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
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
            
            Console.WriteLine($"Serving project on: http://localhost:{ServerPort}");

            while (ServerIsActive)
            {
                // Wait for a request.
                HttpListenerContext context = listener.GetContext(); // WARNING: This is not async.
                
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                
                // Log the requested file.
                Helper.Colorize(ConsoleColor.DarkGray, $"({DateTime.Now:t})", false);
                Helper.Colorize(ConsoleColor.Cyan, $" {request.RawUrl}");

                string requestedFileName = Path.GetFileName(request.RawUrl);
                string requestedFile = !Path.HasExtension(requestedFileName) ? Path.Join(request.RawUrl, "index.html") : request.RawUrl;

                // Return a 404 for files that don't exist.
                if (!File.Exists(Path.Join(site, requestedFile)))
                {
                    response.StatusCode = 404;
                    requestedFileExists = false;
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

                if (extension != ".html" || extension != ".css" || extension != ".js" && requestedFileExists)
                {
                    buffer = File.ReadAllBytes(Path.Join(site, requestedFile));
                }
                else if (requestedFileExists)
                {
                    string responseString = File.ReadAllText(Path.Join(site, requestedFile));
                    buffer = Encoding.UTF8.GetBytes(responseString);
                }
                
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer);
                output.Close();
            }
        }
        
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            if (!ChangedFiles.Contains(e.FullPath))
            {
                ChangedFiles.Add(e.FullPath);
                Rebuild();
            }

            Timer timer = new Timer(100) {AutoReset = false};
            timer.Elapsed += (timerElapsedSender, timerElapsedArgs) =>
            {
                lock (ChangedFiles)
                {
                    ChangedFiles.Remove(e.FullPath);
                }
            };
            
            timer.Start();
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

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

        private void Rebuild()
        {
            // ServerIsActive = false;
            Helper.Colorize(ConsoleColor.DarkGray, $"({DateTime.Now:t}) ", false);
            Helper.Colorize(ConsoleColor.Cyan, "Rebuilding");
            Build.Execute();
            // ServerIsActive = true;
        }
    }
}