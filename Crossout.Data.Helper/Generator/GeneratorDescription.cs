using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Data.Helper
{
    public class GeneratorDescription
    {
        public string SourceFileName { get; set; }

        public string Classname { get; set; }
        public string Namespace { get; set; } = "Crossout.Data.Stats.Main";
        public string ClassFileName { get; set; }
    }
}
