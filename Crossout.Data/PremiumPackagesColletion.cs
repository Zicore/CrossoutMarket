using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Crossout.Data.PremiumPackages;
using NLog;
using Newtonsoft.Json;

namespace Crossout.Data
{
    public class PremiumPackagesColletion
    {
        private Logger Log = LogManager.GetCurrentClassLogger();

        public List<PremiumPackage> Packages { get; } = new List<PremiumPackage>();
        
        public PremiumPackagesColletion()
        {
            
        }

        public void ReadPackages(string directory)
        {
            PremiumPackage package = new PremiumPackage();
            DirectoryInfo packageDir = new DirectoryInfo(directory);

            foreach (var file in packageDir.GetFiles())
            {
                StreamReader sr = new StreamReader(file.FullName);

                string output = sr.ReadToEnd();

                package = JsonConvert.DeserializeObject<PremiumPackage>(output);
                Packages.Add(package);
            }
        }
    }
}
