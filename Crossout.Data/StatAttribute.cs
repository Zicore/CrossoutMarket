using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Data
{
    public class StatAttribute : Attribute
    {
        public const String PowerScoreClasses = "stat-heading-3";

        public string Name { get; set; }
        public int Order { get; set; }
        public bool ShowProgressBar { get; set; }
        public string CustomClasses { get; set; }

        public StatAttribute(string name, int order = 0)
        {
            this.Name = name;
            this.Order = order;
        }
    }
}
