using Crossout.Model.Formatter;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    [TableName("salvagereward")]
    [PrimaryKey("id")]
    public class SalvageRewardPoco
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("raritynumber")]
        public int RarityNumber { get; set; }

        [Column("rewarditem")]
        public int RewardItem { get; set; }

        [Column("rewardamount")]
        public int RewardAmount { get; set; }

        [ResultColumn]
        [ComplexMapping]
        public ItemPoco Item { get; set; }

        [ResultColumn]
        [ComplexMapping]
        public RarityPoco Rarity { get; set; }

        [Ignore]
        public decimal RewardSellPrice { get => Item.SellPrice * (RewardAmount / (decimal)Item.Amount); }

        [Ignore]
        public decimal RewardBuyPrice { get => Item.BuyPrice * (RewardAmount / (decimal)Item.Amount); }

        [Ignore]
        public string FormatRewardSellPrice { get => PriceFormatter.FormatPrice(RewardSellPrice); }

        [Ignore]
        public string FormatRewardBuyPrice { get => PriceFormatter.FormatPrice(RewardBuyPrice); }
    }
}
