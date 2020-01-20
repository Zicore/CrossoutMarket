using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Helper
{
    public static class CacheBuster
    {
        public static string Seconds { get => "t=" + DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds.ToString("0"); }
        public static string Hours { get => "t=" + DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalHours.ToString("0"); }
    }
}
