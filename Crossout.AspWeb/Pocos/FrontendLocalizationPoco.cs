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
        [Column("id")]
        public int Id { get; set; }

        [Column("languagenumber")]
        public int LanguageNumber { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("localization")]
        public string Localization { get; set; }

        [ComplexMapping]
        [ResultColumn]
        public LanguagePoco Language { get; set; }

        [Ignore]
        public string FullName { get => Category + "." + Name; }
    }
}
