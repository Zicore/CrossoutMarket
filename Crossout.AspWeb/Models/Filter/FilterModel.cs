using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Filter
{
    public class FilterModel
    {
        public List<FilterItem> Rarities { get; set; } = new List<FilterItem>();
        public List<FilterItem> Categories { get; set; } = new List<FilterItem>();
        public List<FilterItem> Factions { get; set; } = new List<FilterItem>();
       
        public FilterItem CurrentRarity { get; set; } = new FilterItem();
        public FilterItem CurrentCategory { get; set; } = new FilterItem();
        public FilterItem CurrentFaction { get; set; } = new FilterItem();
        public bool CurrentShowRemovedItems { get; set; }
        public bool CurrentShowMetaItems { get; set; }
        public bool CurrentShowRemovedItemsOnly { get; set; }

        public FilterItem VerifyRarity(string rarity)
        {
            if (rarity != null)
            {
                var item = Rarities.FirstOrDefault(x => x.NameUri == rarity.ToLower());
                if (item != null)
                {
                    item.Active = true;
                    CurrentRarity = item;
                    return item;
                }
            }
            return null;
        }

        public FilterItem VerifyCategory(string category)
        {
            if (category != null)
            {
                var item = Categories.FirstOrDefault(x => x.NameUri == category.ToLower());
                if (item != null)
                {
                    item.Active = true;
                    CurrentCategory = item;
                    return item;
                }
            }
            return null;
        }

        public FilterItem VerifyFaction(string faction)
        {
            if (faction != null)
            {
                var item = Factions.FirstOrDefault(x => x.NameUri == faction.ToLower());
                if (item != null)
                {
                    item.Active = true;
                    CurrentFaction = item;
                    return item;
                }
            }
            return null;
        }

        public bool VerifyRmdItems(string showRemovedItems)
        {
            bool result = false;
            bool.TryParse(showRemovedItems,out result);
            return result;
        }

        public bool VerifyMetaItems(string showMetaItems)
        {
            bool result = false;
            bool.TryParse(showMetaItems, out result);
            return result;
        }
    }
}
