using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace Dagger.Services.Routines
{
    public class Watch : Routine
    {
        private Build Builder { get; }
        private List<String> ChangedFiles { get; } = new List<string>();

        public Watch()
        {
            Builder = new Build();
        }

        public override void Execute()
        {
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

            watcher.IncludeSubdirectories = true;

            // todo: debugging
            watcher.EnableRaisingEvents = true;
            Console.WriteLine($"Watching: {resources}");
            Console.ReadLine();
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
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"({DateTime.Now.ToString("t")}) ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Rebuilding");
                Console.ResetColor();
                Builder.Execute();
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
    }
}