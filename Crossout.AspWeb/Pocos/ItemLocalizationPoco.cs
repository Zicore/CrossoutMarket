using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    [TableName("itemlocalization")]
    [PrimaryKey("id")]
    public class ItemLocalizationPoco
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("languagenumber")]
        public int LanguageNumber { get; set; }

        [Column("itemnumber")]
        public int ItemNumber { get; set; }

        [Column("localizedname")]
        public string LocalizedName { get; set; }
    }
}
