using System;
using Crossout.Web.Services;
using Microsoft.Owin.Hosting;
using NLog;

namespace Crossout.Web
{
    class Program
    {
        static void Main(string[] args)
        {
            WebSettings.Settings.Load();
            WebSettings.Settings.Save(); // Saving defaults
            var url = "http://+:" + WebSettings.Settings.WebserverPort;

            CrossoutDataService.Initialize();

            Logger log = LogManager.GetCurrentClassLogger();

            var rootPath = RootPathProvider.GetRootPathStatic();
            log.Error($"RootPath: {rootPath}");
            log.Error($"SettingsPath: {WebSettings.Settings.FilePath}");

            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Running on {0}", url);
                string command = "";
                do
                {
                    Console.WriteLine("Enter save to save the config file");
                    Console.WriteLine("Enter load to load the config file");
                    Console.WriteLine("Enter exit to shut down the server");
                    command = Console.ReadLine();

                    if (command == "save")
                    {
                        Console.WriteLine("Saving config");
                        WebSettings.Settings.Save();
                    }
                    if (command == "load")
                    {
                        Console.WriteLine("Saving config");
                        WebSettings.Settings.Load();
                    }

                } while (command != "exit" && command != "quit");
            }
        }

        private static void UnhandledExceptionCallback(Exception exception)
        {

        }
    }
}
