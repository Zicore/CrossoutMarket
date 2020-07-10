using System.Collections.Generic;
using System.Linq;
using Crossout.Data;
using Crossout.Model.Items;
using Crossout.AspWeb.Models.View;

namespace Crossout.AspWeb.Models.Items
{
    public class ItemCollection : BaseViewModel, IViewTitle
    {
        public List<Item> Items { get; set; } = new List<Item>();
        public List<Item> AllItems { get; set; } = new List<Item>();
        public List<StatAttribute> StatTypes = new List<StatAttribute>();
        public string ItemList;

        public void CreateStatList()
        {
            foreach (var item in Items)
            {
                if (item.Stats != null)
                {
                    foreach (var stat in item.Stats.SortedStats)
                    {
                        if (!StatTypes.Contains(stat.Stat) && stat.Value != null)
                        {
                            if (!stat.Value.Equals(0) && !stat.Value.Equals(0.0))
                            {
                                StatTypes.Add(stat.Stat);
                            }
                        }
                    }
                }
            }

            StatTypes = StatTypes.OrderBy(x => x.Order).ToList();
        }

        public override string ToString()
        {
            return $"{nameof(Items)}: {Items}";
        }

        public string Title => "Compare";
    }
}
