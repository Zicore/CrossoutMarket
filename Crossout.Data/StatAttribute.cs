using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossout.Data
{
    public enum StatType
    {
        Exposed,
        Hidden,
        Misc
    }

    [JsonObject(ItemTypeNameHandling = TypeNameHandling.None)]
    public class StatAttribute : Attribute
    {
        public const String PowerScoreClasses = "stat-heading-3";

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("showProgressBar")]
        public bool ShowProgressBar { get; set; }

        [JsonProperty("customClasses")]
        public string CustomClasses { get; set; }

        private StatType type = StatType.Exposed;
        [JsonProperty("type")]
        public StatType Type {
            get { return type; }
            set { type = value; }
        }

        public StatAttribute(string name, int order = 0)
        {
            this.Name = name;
            this.Order = order;
        }

        [JsonIgnore]
        public override object TypeId => GetType();
    }
}
