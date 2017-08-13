using Newtonsoft.Json;

namespace Crossout.Data.Stats.Main
{
    public class SingleStat
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("stat")]
        public StatAttribute Stat { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("displayValue")]
        public bool DisplayValue
        {
            get { return Value != null && !Value.Equals(0.0) && !Value.Equals(0); }
        }
    }
}