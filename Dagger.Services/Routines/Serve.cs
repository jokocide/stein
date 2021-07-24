using System;
using System.IO;
using System.Net;

namespace Dagger.Services.Routines
{
    public class Serve : Routine
    {
        private string[] prefixes { get; } = { "http://localhost:8000/" };
        public override void Execute()
        {
            HttpListener listener = new HttpListener();

            foreach (string prefix in prefixes)
                listener.Prefixes.Add(prefix);

            listener.Start();
            Console.WriteLine("Dagger is listening on http://localhost:8000");
            Console.WriteLine();
            
            while (true)
            {
                // Start listening for a request to our prefix addresses.
                HttpListenerContext context = listener.GetContext(); // WARNING: This is not async.
                
                // Request/response established.
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                // Shorthand to refer to the project's site directory.
                string site = Path.Join(Directory.GetCurrentDirectory(), "site");
                
                // To contain the string that will be appended to our HttpListenerResponse object before returning.
                string responseString;
                
                // Logging incoming requests to the console.
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"({DateTime.Now.ToString("t")})");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($" {request.RawUrl}");
                Console.ResetColor();

                switch (request.RawUrl)
                {
                    case "/":
                    case "/index":
                        responseString = File.Exists(Path.Join(site, "index.html"))
                            ? File.ReadAllText(Path.Join(site, "index.html"))
                            : "<HTML><BODY> 404 </BODY></HTML>"; // todo: Create a 404 or something?
                        break;
                    default:
                        responseString = File.Exists(Path.Join(Directory.GetCurrentDirectory(), "site", request.RawUrl))
                            ? File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "site", request.RawUrl))
                            : "<HTML><BODY> 404 </BODY></HTML>"; // todo: Create a 404 or something?
                        break;
                }
                
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);

                output.Close();
            }
        }
    }
}