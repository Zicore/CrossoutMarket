using Crossout.Toolkit.Tools;
using System;

namespace Crossout.Toolkit
{
    class Program
    {
        static void Main(string[] args)
        {
            ToolkitSettings.Settings.Load();
            ToolkitSettings.Settings.Save(); // Saving defaults

            Console.WriteLine("Toolkit started!");

            //APICook.Execute("Data\\Shared\\items.json");
            //LocNameImporter.Execute("Data\\Shared\\items.json");
            //Reparser.Execute();

            Console.ReadKey();
        }
    }
}
