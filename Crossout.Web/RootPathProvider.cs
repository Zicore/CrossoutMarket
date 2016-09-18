using System;
using System.IO;
using Nancy;
using NLog;

namespace Crossout.Web
{
    public class RootPathProvider : IRootPathProvider
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public string GetRootPath()
        {
            
            var path = StaticConfiguration.IsRunningDebug
                ? Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."))
                : AppDomain.CurrentDomain.BaseDirectory;
            return path;
        }
    }
}
