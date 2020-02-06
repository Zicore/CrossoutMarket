using System;
using System.Collections.Generic;
using System.Text;

namespace Crossout.Toolkit.Models.LocNameImporter
{
    class DBLocalization
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public int ItemId { get; set; }
        public string LocName { get; set; }
    }
}
