using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Crossout.Data.Stats
{
    public abstract class PartStatsBase
    {
        protected PartStatsBase(string key)
        {
            this.Key = key;
        }

        public string Key { get; set; }

        public Dictionary<string, SingleStat> Stats { get; } = new Dictionary<string, SingleStat>();
        public List<SingleStat> SortedStats { get; } = new List<SingleStat>();
        
        public void LoadAttributes()
        {
            Stats.Clear();
            SortedStats.Clear();

            var properties = this.GetType().GetProperties();
            foreach (var p in properties)
            {
                var attributes = p.GetCustomAttributes(typeof(StatAttribute), false);
                if (attributes.Length > 0)
                {
                    var statAttrib = (StatAttribute)attributes[0];
                    if (!Stats.ContainsKey(p.Name))
                    {
                        Stats[p.Name] = new SingleStat { Key = p.Name, Stat = statAttrib, Value = p.GetValue(this) };
                    }
                }
            }

            var sortedStats = Stats.Values.OrderBy(x => x.Stat.Order);
            SortedStats.AddRange(sortedStats);
        }
    }
}
