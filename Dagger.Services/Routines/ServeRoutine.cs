using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Watch a Dagger project for changes to the resources directory, and trigger a Build routine as a result.
    /// </summary>
    public class ServeRoutine : Routine
    {
        private BuildRoutine Builder { get; }
        private List<String> ChangedFiles { get; } = new List<string>();
        
        private string[] Prefixes { get; } = { "http://localhost:" };

        private bool Serving = true;
        
        private string Port { get; }

        public ServeRoutine(string port = "8000")
        {
            Builder = new BuildRoutine();
            Port = port;
            Prefixes[0] += $"{port}/";
        }

        public override void Execute()
        {
            HttpListener listener = new HttpListener();

            foreach (string prefix in Prefixes)
                listener.Prefixes.Add(prefix);

            string resources = Path.Join(Directory.GetCurrentDirectory(), "resources");

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
            Console.WriteLine($"Watching: {resources}");
            // Console.ReadLine(); This normally keeps the program alive.
            
            listener.Start();
            Console.WriteLine($"Serving project on: http://localhost:{Port}");
            
            while (Serving)
            {
                // Start listening for a request to our prefix addresses.
                HttpListenerContext context = listener.GetContext(); // WARNING: This is not async.
                
                // Request/response established.
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                
                // Byte array to be returned.
                byte[] buffer = new byte[] { };
                
                // True if file exists on disk, else false.
                bool requestedFileExists = true;

                // Shorthand to refer to the project's site directory.
                string site = Path.Join(Directory.GetCurrentDirectory(), "site");
                
                // Contains the requested file.
                string requestedFile;
                
                // Logging incoming requests to the console.
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"({DateTime.Now.ToString("t")})");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($" {request.RawUrl}");
                Console.ResetColor();

                string preRequestedFile = Path.GetFileName(request.RawUrl);

                if (!Path.HasExtension(preRequestedFile))
                {
                    requestedFile = Path.Join(request.RawUrl, "index.html");
                }
                else
                {
                    requestedFile = request.RawUrl;
                }

                // Return a 404 for files that don't exist.
                if (!File.Exists(Path.Join(site, requestedFile)))
                {
                    response.StatusCode = 404;
                    requestedFileExists = false;
                    string responseString = "<HTML><BODY>404</BODY></HTML>";
                    buffer = Encoding.UTF8.GetBytes(responseString);
                }

                // The requested extension is used to set an appropriate content type.
                string extension = Path.GetExtension(requestedFile);
                
                // Image content types.
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
                
                // Set an appropriate content length.
                response.ContentLength64 = buffer.Length;
                
                // Return the byte array via response.outputStream.
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
            Serving = false;
            Helper.Colorize(ConsoleColor.DarkGray, $"({DateTime.Now.ToString("t")}) ", false);
            Helper.Colorize(ConsoleColor.Cyan, "Rebuilding");
            Builder.Execute();
            Serving = true;
        }
    }
}