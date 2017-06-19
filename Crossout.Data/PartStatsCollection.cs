using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crossout.Data
{
    public class PartStatsCollection
    {
        private readonly string statsPattern = @"Def\.(?<name>[\w]+)\.(?<field>[\w]+)=(?<value>.+)";

        private readonly Regex statsRegex;

        public PartStatsCollection()
        {
            statsRegex = new Regex(statsPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        public Dictionary<string, PartStats> Items { get; } = new Dictionary<string, PartStats>();

        public void ReadStats(string file)
        {
            Items.Clear();
            
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
                        AddMatch(line);
                    }
                }
            }
        }

        private void AddMatch(string line)
        {
            var match = statsRegex.Match(line);
            if (match.Success)
            {
                if (match.Groups["name"].Success)
                {
                    var name = match.Groups["name"].Value;
                    if (!Items.ContainsKey(name))
                    {
                        Items.Add(name, new PartStats(name));
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

        private void SetField(PartStats stats,string field, string value)
        {
            var propertyInfo = typeof(PartStats).GetProperty(field);
            object typedValue = Convert.ChangeType(value, propertyInfo.PropertyType);
            propertyInfo.SetValue(stats, typedValue);
        }
    }
}
