using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossout.AspWeb.Models.API.v2
{
    public class ApiRarityEntry : ApiEntryBase
    {
        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("primarycolor")]
        public string PrimaryColor { get; set; }

        [JsonProperty("secondarycolor")]
        public string SecondaryColor { get; set; }
    }
}
