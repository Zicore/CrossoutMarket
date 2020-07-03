using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    [TableName("rarity")]
    [PrimaryKey("id")]
    public class RarityPoco
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("order")]
        public int Order { get; set; }

        [Column("primarycolor")]
        public string PrimaryColor { get; set; }

        [Column("secondarycolor")]
        public string SecondaryColor { get; set; }
    }
}
