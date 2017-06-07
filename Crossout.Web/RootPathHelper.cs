using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Web
{
    public class RootPathHelper
    {
        public static bool ImageExists(string path)
        {
            return File.Exists(System.IO.Path.Combine(RootPathProvider.GetRootPathStatic(), "img", "items", path));
        }

        public static string AbsolutePath(string relativePath)
        {
            return System.IO.Path.Combine(RootPathProvider.GetRootPathStatic(), relativePath);
        }
    }
}
