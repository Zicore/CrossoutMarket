using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crossout.Toolkit.Models.APICook
{
    public class ItemDef
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("subtype")]
        public string SubType { get; set; }

        [JsonProperty("rarity")]
        public int Rarity { get; set; }

        [JsonProperty("locName")]
        public string LocName { get; set; }
    }
}
