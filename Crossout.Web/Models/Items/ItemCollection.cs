using System.Collections.Generic;
using Crossout.Web.Helper;
using Crossout.Data;

namespace Crossout.Model.Items
{
    public class ItemCollection : IViewTitle
    {
        public List<Item> Items { get; set; } = new List<Item>();
        public List<StatAttribute> StatTypes = new List<StatAttribute>();

        public void CreateStatList()
        {
            foreach (var item in Items)
            {
                foreach (var stat in item.Stats.SortedStats)
                {
                    if (!StatTypes.Contains(stat.Stat))
                    {
                        StatTypes.Add(stat.Stat);
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"{nameof(Items)}: {Items}";
        }

        public string Title => "Compare";
    }
}
