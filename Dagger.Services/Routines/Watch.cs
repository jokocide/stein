using System;
using System.IO;

namespace Dagger.Services.Routines
{
    public class Watch : Routine
    {
        private Build Builder { get; }

        public Watch()
        {
            Builder = new Build();
        }

        public override void Execute()
        {
            string cwd = Directory.GetCurrentDirectory();

            FileSystemWatcher watcher = new FileSystemWatcher(cwd)
            {
                NotifyFilter = NotifyFilters.Attributes
                               | NotifyFilters.CreationTime
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.FileName
                               | NotifyFilters.LastWrite
                               | NotifyFilters.Security
                               | NotifyFilters.Size
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
            // watcher.EnableRaisingEvents = true;
            Console.WriteLine($"Watching: {Directory.GetCurrentDirectory()}");

            while (true)
            {
                watcher.WaitForChanged(WatcherChangeTypes.Changed);
            }
        }
        
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"({DateTime.Now.ToString("t")}) ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Changed was detected.");
            Console.ResetColor();
            Builder.Execute();
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