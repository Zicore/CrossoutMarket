using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crossout.Toolkit.Models.LocNameImporter
{
    public class LocNameFile
    {
        [JsonProperty("err")]
        public string Err { get; set; }

        [JsonProperty("res")]
        public List<LocItem> Res { get; set; }
    }
}
