using Crossout.Model;
using Crossout.Model.Formatter;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Drafts.Snipe
{
    public class MarketEntry
    {
        public int Id { get; set; }
        public int ItemNumber { get; set; }
        public int SellPrice { get; set; }
        public int BuyPrice { get; set; }
        public int SellOffers { get; set; }
        public int BuyOrders { get; set; }
        public DateTime Timestamp { get; set; }
        public string ItemName { get; set; }
        public string ItemLocalizedName { get; set; }

        public string FormatSellPrice { get => PriceFormatter.FormatPrice(SellPrice); }
        public string FormatBuyPrice { get => PriceFormatter.FormatPrice(BuyPrice); }

        public string FormatTimestamp { get => Timestamp.ToString("yyyy-MM-dd HH:mm:ss"); }

        public void Create(object[] row)
        {
            int i = 0;
            Id = row[i++].ConvertTo<int>();
            ItemNumber = row[i++].ConvertTo<int>();
            SellPrice = row[i++].ConvertTo<int>();
            BuyPrice = row[i++].ConvertTo<int>();
            SellOffers = row[i++].ConvertTo<int>();
            BuyOrders = row[i++].ConvertTo<int>();
            Timestamp = row[i++].ConvertTo<DateTime>();
            ItemName = row[i++].ConvertTo<string>();
            ItemLocalizedName = row[i++].ConvertTo<string>();
        }
    }
}
