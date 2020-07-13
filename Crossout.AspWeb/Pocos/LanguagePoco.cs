using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    [TableName("language")]
    [PrimaryKey("id")]
    public class LanguagePoco
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("shortname")]
        public string ShortName { get; set; }
    }
}
