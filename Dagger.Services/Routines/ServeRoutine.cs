using System;
using System.IO;
using System.Net;

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

                // Shorthand to refer to the project's site directory.
                string site = Path.Join(Directory.GetCurrentDirectory(), "site");
                
                // Contains the requested file.
                string requestedFile;
                
                // To contain the string that will be appended to our HttpListenerResponse object before returning.
                string responseString = null;
                
                // Contains other junk
                
                // Logging incoming requests to the console.
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"({DateTime.Now.ToString("t")})");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($" {request.RawUrl}");
                Console.ResetColor();

                if (request.RawUrl == "/")
                {
                    requestedFile = "index.html";
                }
                else
                {
                    requestedFile = request.RawUrl;
                }

                if (!File.Exists(Path.Join(site, requestedFile)))
                {
                    response.StatusCode = 404;
                }

                if (Path.GetExtension(requestedFile) == ".png")
                {
                    response.ContentType = "image/png";
                    byte[] responseBytes =
                        File.ReadAllBytes(Path.Join(Directory.GetCurrentDirectory(), "site", requestedFile));
                    test(responseBytes, response);
                }
                else
                {
                    responseString =
                        File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "site", requestedFile));
                }

                // switch (request.RawUrl)
                // {
                //     case "/":
                //     case "/index":
                //         // responseString = File.Exists(Path.Join(site, "index.html"))
                //         //     ? File.ReadAllText(Path.Join(site, "index.html"))
                //         //     : "<HTML><BODY> 404 </BODY></HTML>"; // todo: Create a 404 or something?
                //         break;
                //     default:
                //         responseString = File.Exists(Path.Join(Directory.GetCurrentDirectory(), "site", request.RawUrl))
                //             ? File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "site", request.RawUrl))
                //             : "<HTML><BODY> 404 </BODY></HTML>"; // todo: Create a 404 or something?
                //         // Wtf(response, request.RawUrl);
                //         break;
                // }

                if (responseString != null)
                {
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }

                // output.Close();
            }
        }

        private void test(byte[] cockass, HttpListenerResponse res)
        {
            Stream output = res.OutputStream;
            output.Write(cockass);
            output.Close();
        }
    }
}