using Newtonsoft.Json;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    [TableName("frontendlocalization")]
    [PrimaryKey("id")]
    public class FrontendLocalizationPoco
    {
        [JsonProperty("id")]
        [Column("id")]
        public int Id { get; set; }

        [JsonProperty("languagenumber")]
        [Column("languagenumber")]
        public int LanguageNumber { get; set; }

        [JsonProperty("category")]
        [Column("category")]
        public string Category { get; set; }

        [JsonProperty("name")]
        [Column("name")]
        public string Name { get; set; }

        [JsonProperty("localization")]
        [Column("localization")]
        public string Localization { get; set; }

        [JsonIgnore]
        [ComplexMapping]
        [ResultColumn]
        public LanguagePoco Language { get; set; }

        [JsonProperty("fullname")]
        [Ignore]
        public string FullName { get => Category + "." + Name; }
    }
}
