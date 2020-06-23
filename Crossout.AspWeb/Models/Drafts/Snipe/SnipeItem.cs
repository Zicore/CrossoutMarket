using Crossout.Model.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Drafts.Snipe
{
    public class SnipeItem
    {
        public int Id { get; set; }
        public string Image { get => Id + ".png"; }
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public string AvailableName { get => LocalizedName ?? Name; }
        public List<MarketEntry> MarketEntries { get; set; } = new List<MarketEntry>();

        public bool ImageExists { get; set; }

        public decimal Margin { get => CalculateMargin(); }
        public string FormatMargin { get => PriceFormatter.FormatPrice(Margin); }
        
        public decimal ROI { get => HighEntry.SellPrice != 0 ? Margin / HighEntry.SellPrice : 0m ; }
        public string FromatROI { get => PriceFormatter.FormatRatio(ROI); }

        public MarketEntry HighEntry { get; set; }
        public MarketEntry LowEntry { get; set; }
        public MarketEntry CurrentEntry { get; set; }

        public void CalculatePriceEdge()
        {
            CurrentEntry = MarketEntries.First();
            foreach (var entry in MarketEntries)
            {
                if (LowEntry == null || entry.SellPrice < LowEntry.SellPrice)
                {
                    LowEntry = entry;
                }
                if (HighEntry == null || entry.SellPrice > HighEntry.SellPrice)
                {
                    HighEntry = entry;
                }
            }
        }

        private decimal CalculateMargin()
        {
            decimal newerSellPrice = CurrentEntry.SellPrice;
            decimal olderSellPrice = HighEntry.SellPrice;
            decimal newerBuyPrice = CurrentEntry.BuyPrice;
            if (newerBuyPrice < newerSellPrice)
            {
                return olderSellPrice - newerSellPrice - (olderSellPrice * 0.1m);
            }
            else
            {
                return 0m;
            }
        }
    }
}
