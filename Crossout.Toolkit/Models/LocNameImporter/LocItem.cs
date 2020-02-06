using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crossout.Toolkit.Models.LocNameImporter
{
    public class LocItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rarity")]
        public string Rarity { get; set; }

        [JsonProperty("subtype")]
        public string Subtype { get; set; }

        [JsonProperty("enUiType")]
        public string EnUiType { get; set; }

        [JsonProperty("ruUiType")]
        public string RuUiType { get; set; }

        [JsonProperty("enName")]
        public string EnName { get; set; }

        [JsonProperty("ruName")]
        public string RuName { get; set; }
    }
}
