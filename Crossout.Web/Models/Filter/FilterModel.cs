using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Web.Models.Filter
{
    public class FilterModel
    {
        public List<FilterItem> Rarities { get; set; } = new List<FilterItem>();
        public List<FilterItem> Categories { get; set; } = new List<FilterItem>();

        public FilterItem CurrentRarity { get; set; } = new FilterItem();
        public FilterItem CurrentCategory { get; set; } = new FilterItem();

        public FilterItem VerifyRarity(string rarity)
        {
            var item = Rarities.FirstOrDefault(x => x.NameUri == rarity);
            if (item != null)
            {
                item.Active = true;
                CurrentRarity = item;
                return item;
            }
            return null;
        }

        public FilterItem VerifyCategory(string category)
        {
            var item = Categories.FirstOrDefault(x => x.NameUri == category);
            if (item != null)
            {
                item.Active = true;
                CurrentCategory = item;
                return item;
            }
            return null;
        }
    }
}
