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
    /// <summary>
    /// Represents a Routine that can be used to make a project available at localhost on 
    /// any valid port number, and automatically rebuild the project when a change is detected.
    /// </summary>
    public sealed class ServeRoutine : Routine
    {
        /// <summary>
        /// Initializes a new instance of the ServeRoutine class with the given configuration, engine and port.
        /// </summary>
        /// <param name="config">A Configuration object used to influence behavior of the routine.</param>
        /// <param name="engine">
        /// The desired templating engine. Passed to a new instance of
        /// the BuildRoutine class when a file is changed.
        /// </param>
        /// <param name="port">The port to host the server on.</param>
        public ServeRoutine(Configuration config, IEngine engine, string port = "8000")
        {
            Config = config;
            Engine = engine;
            Port = port;

            Listener.Prefixes.Add($"http://localhost:{port}/");

            // FileSystemWatcher setup
            Watcher.IncludeSubdirectories = true;
            Watcher.NotifyFilter =
                NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastWrite
                | NotifyFilters.Size;
            Watcher.Changed += HandleEvent;
            Watcher.Deleted += HandleEvent;
            Watcher.Renamed += HandleEvent;
            Watcher.Error += HandleError;
            Watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Serve a project at a local address.
        /// </summary>
        public override void Execute()
        {
            Listener.Start();
            Console.WriteLine($"Serving {Path.GetFileName(Directory.GetCurrentDirectory())} on http://localhost:{Port}");

            while (true)
            {
                HttpListenerContext context = Listener.GetContext();

                byte[] buffer = Array.Empty<byte>();

                string requestedFileName = Path.GetFileName(context.Request.RawUrl);

                string requestedFile = !Path.HasExtension(requestedFileName)
                    ? Path.Join(context.Request.RawUrl, "index.html")
                    : context.Request.RawUrl;

                if (File.Exists(Path.Join(PathService.GetSitePath(), requestedFile)))
                {
                    buffer = File.ReadAllBytes(Path.Join(PathService.GetSitePath(), requestedFile));
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
                    Console.Write($"{context.Request.RawUrl} ");
                    StringService.ColorizeLine("404", ConsoleColor.DarkRed);
                    context.Response.StatusCode = 404;
                }

                context.Response.OutputStream.Close();
            }
        }

        private string Port { get; }

        private Configuration Config { get; }

        private IEngine Engine { get; }

        private HttpListener Listener { get; } = new();

        private FileSystemWatcher Watcher { get; } = new(PathService.GetResourcesPath());

        /// <summary>
        /// Temporarily store the name of files that have emitted an event 
        /// here, necessary to avoid multiple event emits from a single change.
        /// </summary>
        private List<string> Cache { get; } = new();

        private void HandleEvent(object sender, FileSystemEventArgs e)
        {
            if (Cache.Contains(e.FullPath)) return;
            Cache.Add(e.FullPath);

            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                new BuildRoutine(Config, Engine).Execute();
                Message.Print();
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

        private void HandleError(object sender, ErrorEventArgs args)
        {
            Message.Log(Message.ServerRestartRequired(args));
            return;
        }
    }
}