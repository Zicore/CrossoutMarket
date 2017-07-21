using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Data.Generator
{
    public class StatsGenerator
    {
        public static string GeneratorVersion = "0.4";

        readonly StatsReader statsReader = new StatsReader();


        public string SourceFileDirectory { get; set; }
        public string DestinationFileDirectory { get; set; }

        public void Generate(List<GeneratorDescription> items)
        {
            foreach (var description in items)
            {
                var sourceFilePath = Path.Combine(SourceFileDirectory, description.SourceFileName);
                var fields = statsReader.ReadFields(sourceFilePath);
                var stringBuilderClass = GenerateClass(fields.Values, description);
                Save(description.ClassFileName, stringBuilderClass);
            }
        }

        public void Save(string fileName, StringBuilder sb)
        {
            if (!Directory.Exists(DestinationFileDirectory))
            {
                Directory.CreateDirectory(DestinationFileDirectory);
            }
            var destinationPath = Path.Combine(DestinationFileDirectory, fileName);
            File.WriteAllText(destinationPath, sb.ToString());
        }

        public static StringBuilder GenerateClass(IEnumerable<FieldHelper> fields,GeneratorDescription description)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {description.Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    // *** Generator: v{GeneratorVersion} This class is automatically generated, please use the non generated variant of this class ***");
            sb.AppendLine($"    public partial class {description.Classname} : PartStatsBase");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {description.Classname}(string key) : base(key) {{ }}");

            foreach (var f in fields)
            {
                string name = f.Name;

                if (name == "class")
                {
                    name = "@class";
                }

                sb.Append($"        public {f.GetTypeString()} {name} {{get;set;}}");
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb;
        }
    }
}
