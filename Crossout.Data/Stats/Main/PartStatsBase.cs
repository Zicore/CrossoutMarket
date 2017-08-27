using System.Collections.Generic;
using System.Linq;

namespace Crossout.Data.Stats.Main
{
    public abstract class PartStatsBase
    {
        protected PartStatsBase(string key)
        {
            this.Key = key;
        }
        
        public string Key { get; set; }

        public Dictionary<string, object> Fields { get; } = new Dictionary<string, object>();
        public Dictionary<string, SingleStat> Stats { get; } = new Dictionary<string, SingleStat>();
        public List<SingleStat> SortedStats { get; } = new List<SingleStat>();
        
        public void LoadAttributes()
        {
            Stats.Clear();
            SortedStats.Clear();
            Fields.Clear();

            var properties = this.GetType().GetProperties();
            foreach (var p in properties)
            {
                var value = p.GetValue(this);
                Fields[p.Name] = value;
                var attributes = p.GetCustomAttributes(typeof(StatAttribute), false);
                if (attributes.Length > 0)
                {
                    var statAttrib = (StatAttribute)attributes[0];
                    if (!Stats.ContainsKey(p.Name))
                    {
                        Stats[p.Name] = new SingleStat { Key = p.Name, Stat = statAttrib, Value = value};
                    }
                }
            }

            var sortedStats = Stats.Values.OrderBy(x => x.Stat.Order);
            SortedStats.AddRange(sortedStats);
        }

        public override string ToString()
        {
            return Key;
        }
    }
}
