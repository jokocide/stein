using System;
using System.IO;
using Stein.Models;
using Stein.Services;

namespace Stein.Routines
{
    public sealed class NewRoutine : Routine
    {
        public static NewRoutine GetDefault => new NewRoutine();

        public override void Execute()
        {
            if (File.Exists("stein.json"))
            {
                MessageService.Log(new Message("A stein.json already exists in target directory.", Message.InfoType.Error));
                MessageService.Print(true);
            }

            File.WriteAllText("stein.json", new JsonService().Serialize(new Configuration()));

            Directory.CreateDirectory(Path.Join("resources", "pages"));
            Directory.CreateDirectory(Path.Join("resources", "templates", "partials"));
            Directory.CreateDirectory(Path.Join("resources", "collections"));
            Directory.CreateDirectory(Path.Join("resources", "public"));
            Directory.CreateDirectory("site");

            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize("Created project ", ConsoleColor.White, false);
            string projectName = Path.GetFileName(Directory.GetCurrentDirectory());
            StringService.Colorize($"'{projectName}'", ConsoleColor.Gray, true);
        }
    }
}