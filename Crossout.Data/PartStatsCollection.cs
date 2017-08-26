using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Crossout.Data.Stats;
using Crossout.Data.Stats.Main;
using NLog;

namespace Crossout.Data
{
    public class PartStatsCollection
    {
        private Logger Log = LogManager.GetCurrentClassLogger();

        public Dictionary<string, PartStatsBase> Items { get; } = new Dictionary<string, PartStatsBase>();

        private readonly string statsPattern = @"Def\.(?<name>[\w]+)\.(?<field>[^=]+)=(?<value>.+)";
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
                    name = name.ToLowerInvariant();

                    if (!Items.ContainsKey(name))
                    {
                        Items.Add(name, (PartStatsBase)Activator.CreateInstance(typeof(T), name)); // Important: We create a instance based on the Main Type here to populate properties later.
                    }
                    var stats = Items[name];
                    if (match.Groups["field"].Success && match.Groups["value"].Success)
                    {
                        var field = match.Groups["field"].Value;
                        var value = match.Groups["value"].Value;

                        field = StatsReader.TranslateFieldToCsharp(field);

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
            if (propertyInfo != null)
            {
                try
                {
                    object typedValue = Convert.ChangeType(value, propertyInfo.PropertyType, CultureInfo.InvariantCulture);
                    propertyInfo.SetValue(stats, typedValue);
                }
                catch(Exception ex)
                {
                    Log.Warn($"Cannot change type of value for field. Stats: {stats} field: {field} value: {value} type: {propertyInfo.PropertyType}");
                    Log.Trace(ex);
                }
            }
            else
            {
                Log.Warn($"Cannot find field. Stats: {stats} field: {field} value: {value}");
            }
        }
    }
}
