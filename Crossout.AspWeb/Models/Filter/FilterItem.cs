using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Filter
{
    public class FilterItem
    {
        public int Id { get; set; }
        public String Name { get; set; } = "";

        public String NameUri
        {
            get { return Name.ToLower(); }
        }

        public bool Active { get; set; }

        public static FilterItem Create(object[] row)
        {
            int i = 0;
            FilterItem item = new FilterItem
            {
                Id = Convert.ToInt32(row[i++]),
                Name = Convert.ToString(row[i++]),
            };
            return item;
        }
    }
}
