using Crossout.Model.Items;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.AspWeb.Models.View;

namespace Crossout.AspWeb.Models.General
{
    public class HtmlExportModel : BaseViewModel, IViewTitle
    {
        public List<Item> Items { get; set; }
        public FilterModel FilterModel { get; set; } = new FilterModel();
        public StatusModel Status { get; set; } = new StatusModel();

        public bool ShowTable { get; set; }
        public bool ShowImage { get; set; }
        public bool ShowName { get; set; }
        public bool ShowRarity { get; set; }
        public bool ShowFaction { get; set; }
        public bool ShowCategory { get; set; }
        public bool ShowType { get; set; }
        public bool ShowPopulartiy { get; set; }
        public bool ShowSellPrice { get; set; }
        public bool ShowSellOffers { get; set; }
        public bool ShowBuyPrice { get; set; }
        public bool ShowBuyOrders { get; set; }
        public bool ShowMargin { get; set; }
        public bool ShowLastUpdate { get; set; }
        public bool ShowCraftingCostSell { get; set; }
        public bool ShowCraftingCostBuy { get; set; }
        public bool ShowCraftingMargin { get; set; }
        public bool ShowCraftVsBuy { get; set; }
        public bool ShowLink { get; set; }
        public bool ShowId { get; set; }

        public string Title => "HTML Export";
    }
}
