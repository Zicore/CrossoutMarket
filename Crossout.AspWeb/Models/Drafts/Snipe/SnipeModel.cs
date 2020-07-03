using Crossout.AspWeb.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Drafts.Snipe
{
    public class SnipeModel : IViewTitle
    {
        public string Title => "Item Sniper";

        public List<SnipeItem> SnipeItems { get; set; } = new List<SnipeItem>();

        public string FormatTimestamp { get => SnipeItems.First().MarketEntries.First().FormatTimestamp; }
    }
}
