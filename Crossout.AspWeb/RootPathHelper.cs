using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace Crossout.AspWeb
{
    public class RootPathHelper
    {
        public RootPathHelper(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        public bool ImageExists(string path)
        {
            return File.Exists(Path.Combine(WebHostEnvironment.WebRootPath, "img", "items", path));
        }

        public bool HighResImageExists(string path)
        {
            return File.Exists(Path.Combine(WebHostEnvironment.WebRootPath, "img", "items-highres", path));
        }
    }
}
