using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Filter
{
    public class RarityItem
    {
        public int Id { get; set; }
        public String Name { get; set; } = "";
        public int Order { get; set; }
        public String PrimaryColor { get; set; } = "";
        public String SecondaryColor { get; set; } = "";

        public String NameUri
        {
            get { return Name.ToLower(); }
        }

        public bool Active { get; set; }

        public static RarityItem Create(object[] row)
        {
            int i = 0;
            RarityItem item = new RarityItem
            {
                Id = Convert.ToInt32(row[i++]),
                Name = Convert.ToString(row[i++]),
                Order = Convert.ToInt32(row[i++]),
                PrimaryColor = Convert.ToString(row[i++]),
                SecondaryColor = Convert.ToString(row[i++])
            };
            return item;
        }
    }
}
