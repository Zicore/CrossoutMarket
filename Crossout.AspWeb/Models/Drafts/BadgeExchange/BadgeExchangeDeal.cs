using Crossout.Model;
using Crossout.Model.Formatter;
using Crossout.Model.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Drafts.BadgeExchange
{
    public class BadgeExchangeDeal
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public int RewardItemId { get; set; }
        public int BadgeCost { get; set; }
        public int RewardAmount { get; set; }
        public Item RewardItem { get; set; }
        public DateTime LastBeginActive { get; set; }

        public decimal Profit { get => RewardItem.SellPrice / 100 * 0.9m * RewardAmount / RewardItem.Amount; }
        public decimal ProfitPerBadge { get => Profit / BadgeCost; }

        public string FormatLastBeginActive { get => LastBeginActive.ToString("yyyy-MM-dd HH:mm:ss"); }
        public string FormatProfit { get => PriceFormatter.FormatResultPrice(Profit); }
        public string FormatProfitPerBadge { get => PriceFormatter.FormatProfitPerBadgePrice(ProfitPerBadge); }

        public void Create(object[] row)
        {
            int i = 0;
            Id = row[i++].ConvertTo<int>();
            RewardItemId = row[i++].ConvertTo<int>();
            RewardAmount = row[i++].ConvertTo<int>();
            BadgeCost = row[i++].ConvertTo<int>();
            Active = row[i++].ConvertTo<bool>();
            LastBeginActive = row[i++].ConvertTo<DateTime>();
        }
    }
}
