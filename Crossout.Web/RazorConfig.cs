using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.ViewEngines.Razor;

namespace Crossout.Web
{
    public class RazorConfig : IRazorConfiguration
    {
        public IEnumerable<string> GetAssemblyNames()
        {
            yield return "Zicore.Settings.Json";
            yield return "DatabaseConnector";
            yield return "Crossout.Model";
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield return "Zicore.Settings.Json";
            yield return "DatabaseConnector";
            yield return "Crossout.Model";
        }

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }
    }
}
