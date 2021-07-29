using System;
using System.IO;
using System.Net;
using System.Text;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Make a Dagger project available for testing at the specified port, or 8000 if no port is given.
    /// </summary>
    public class ServeRoutine : Routine
    {
        private string[] Prefixes { get; } = { "http://localhost:" };
        private string Port { get; }
        
        public ServeRoutine(string port = "8000")
        {
            Port = port;
            Prefixes[0] += $"{port}/";
        }
        
        public override void Execute()
        {
            HttpListener listener = new HttpListener();

            foreach (string prefix in Prefixes)
                listener.Prefixes.Add(prefix);

            listener.Start();
            Console.WriteLine($"Serving project on: http://localhost:{Port}");
            
            while (true)
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

                // Request.RawUrl is normalized and stored in requestedFile.
                if (request.RawUrl == "/" || request.RawUrl == "/index")
                {
                    requestedFile = "index.html";
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
    }
}