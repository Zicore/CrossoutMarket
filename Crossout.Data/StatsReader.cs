using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crossout.Data
{
    public enum FieldHelperType
    {
        Undefined,
        Bool,
        Float,
        Int,
        String
    }

    public class FieldHelper
    {
        public string Name { get; set; }
        public FieldHelperType Type { get; set; } = FieldHelperType.Undefined;
        public string Value { get; set; }

        public string GetTypeString()
        {
            switch (Type)
            {
                case FieldHelperType.Undefined:
                    return "undefined";
                case FieldHelperType.Bool:
                    return "bool";
                case FieldHelperType.Float:
                    return "double";
                case FieldHelperType.Int:
                    return "int";
                case FieldHelperType.String:
                    return "string";
                default: return "string";
            }

        }
    }

    public class StatsReader
    {
        public StatsReader()
        {

        }

        private readonly string statsPattern = @"Def\.(?<name>[\w]+)\.(?<field>[\w]+)=(?<value>.+)";

        // Function to generate UNIQUE fields for PartStats.cs
        public void ReadFields(string file)
        {
            Dictionary<string, FieldHelper> fields = new Dictionary<string, FieldHelper>();

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

                                if (value != "{}")
                                {
                                    if (!fields.ContainsKey(field))
                                    {
                                        fields.Add(field, new FieldHelper { Name = field });
                                    }
                                    var fieldHelper = fields[field];

                                    if (fieldHelper.Type == FieldHelperType.Undefined)
                                    {
                                        fieldHelper.Type = FieldHelperType.String; // Default
                                    }
                                    bool boolValue;
                                    if (Boolean.TryParse(value, out boolValue))
                                    {
                                        fieldHelper.Type = FieldHelperType.Bool;
                                    }

                                    if (fieldHelper.Type != FieldHelperType.Float)
                                    {
                                        int intValue;
                                        if (int.TryParse(value, out intValue))
                                        {
                                            fieldHelper.Type = FieldHelperType.Int;
                                        }
                                    }

                                    float floatValue;
                                    if (value.Contains(".") &&
                                        float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture,
                                            out floatValue))
                                    {
                                        fieldHelper.Type = FieldHelperType.Float;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();

            foreach (var f in fields.Values)
            {
                sb.Append($"public {f.GetTypeString()} {f.Name} {{get;set;}}");
                sb.AppendLine();
            }

            // This helps to generate Fields for new PartStats
            Debug.WriteLine(sb.ToString());

        }
    }
}
