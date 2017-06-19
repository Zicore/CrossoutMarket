using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crossout.Data
{
    public class StatsReader
    {
        public StatsReader()
        {

        }

        private readonly string statsPattern = @"Def\.(?<name>[\w]+)\.(?<field>[\w]+)=(?<value>.+)";

        // Function to generate UNIQUE fields for PartStats.cs
        public void ReadFields(string file)
        {
            Dictionary<string, string> fields = new Dictionary<string, string>();
            Regex regex = new Regex(statsPattern, RegexOptions.Singleline);
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
                        var match = regex.Match(line);
                        if (match.Success)
                        {
                            if (match.Groups["field"].Success)
                            {
                                var field = match.Groups["field"].Value;
                                var value = match.Groups["value"].Value;
                                if (!fields.ContainsKey(field))
                                {
                                    fields.Add(field, value);
                                }
                            }
                        }
                    }
                }
            }
            foreach (var field in fields)
            {
                Debug.WriteLine($"{field.Key}={field.Value}");
            }
        }
    }
}
