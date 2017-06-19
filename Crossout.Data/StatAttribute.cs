using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Data
{
    public class StatAttribute : Attribute
    {
        public string Name { get; set; }
        public int Order { get; set; }

        public StatAttribute(string name, int order = 0)
        {
            this.Name = name;
            this.Order = order;
        }
    }
}
