using System.Collections.Generic;
using Crossout.Web.Helper;
using Crossout.Data;
using System.Linq;

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
                if (item.Stats != null)
                {
                    foreach (var stat in item.Stats.SortedStats)
                    {
                        if (!StatTypes.Contains(stat.Stat))
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
