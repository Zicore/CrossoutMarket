using System;
using System.Collections.Generic;
using System.Text;

namespace Crossout.Toolkit.Models.APICook
{
    public class MarketItem
    {
        public string time { get; set; }
        public string def { get; set; }
        public int? sellorders { get; set; }
        public int? buyorders { get; set; }
        public int? minsellprice { get; set; }
        public int? maxbuyprice { get; set; }
    }
}
