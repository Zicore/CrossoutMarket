using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace Crossout.Web
{
    public class RootPathHelper
    {
        public RootPathHelper(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        public bool ImageExists(IWebHostEnvironment hostingEnvironment, string path)
        {
            return File.Exists(System.IO.Path.Combine(hostingEnvironment.WebRootPath, "img", "items", path));
        }

        public bool HighResImageExists(IWebHostEnvironment hostingEnvironment, string path)
        {
            return File.Exists(System.IO.Path.Combine(hostingEnvironment.WebRootPath, "img", "items-highres", path));
        }
    }
}
