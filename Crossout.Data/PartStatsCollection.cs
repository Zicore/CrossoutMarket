using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Crossout.Data.Stats;

namespace Crossout.Data
{
    public class PartStatsCollection
    {
        public Dictionary<string, PartStatsBase> Items { get; } = new Dictionary<string, PartStatsBase>();

        private readonly string statsPattern = @"Def\.(?<name>[\w]+)\.(?<field>[\w]+)=(?<value>.+)";
        private readonly Regex statsRegex;
        
        public PartStatsCollection()
        {
            statsRegex = new Regex(statsPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }
        
        private void LoadAttributes()
        {
            foreach (var stat in Items)
            {
                stat.Value.LoadAttributes();
            }
        }

        public void ReadStats<T>(string file) where T : PartStatsBase
        {
            using (StreamReader sr = new StreamReader(file))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line) || line.StartsWith("--"))
                    {
                        continue;
                    }

                    if (line.StartsWith("Def."))
                    {
                        AddMatch<T>(line);
                    }
                }
            }

            LoadAttributes();
        }
        
        private void AddMatch<T>(string line) where T : PartStatsBase
        {
            var match = statsRegex.Match(line);
            if (match.Success)
            {
                if (match.Groups["name"].Success)
                {
                    var name = match.Groups["name"].Value;
                    if (!Items.ContainsKey(name))
                    {
                        Items.Add(name, (PartStatsBase)Activator.CreateInstance(typeof(T), name)); // Important: We create a instance based on the Main Type here to populate properties later.
                    }
                    var stats = Items[name];
                    if (match.Groups["field"].Success && match.Groups["value"].Success)
                    {
                        var field = match.Groups["field"].Value;
                        var value = match.Groups["value"].Value;

                        if (value != "{}")
                        {
                            SetField(stats, field, value);
                        }
                    }
                }
            }
        }

        private void SetField(PartStatsBase stats, string field, string value)
        {
            var propertyInfo = stats.GetType().GetProperty(field);
            object typedValue = Convert.ChangeType(value, propertyInfo.PropertyType, CultureInfo.InvariantCulture);
            propertyInfo.SetValue(stats, typedValue);
        }
    }
}
