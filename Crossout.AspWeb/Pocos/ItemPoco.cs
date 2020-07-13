using Crossout.Model.Formatter;
using Crossout.Model.Items;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    public enum PriceType
    {
        Sell,
        Buy
    }

    [TableName("item")]
    [PrimaryKey("id")]
    public class ItemPoco
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("externalKey")]
        public string ExternalKey { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("raritynumber")]
        public int RarityNumber { get; set; }

        [Column("categorynumber")]
        public int CategoryNumber { get; set; }

        [Column("typenumber")]
        public int TypeNumber { get; set; }

        [Column("sellprice")]
        public int SellPrice { get; set; }

        [Column("buyprice")]
        public int BuyPrice { get; set; }

        [Column("selloffers")]
        public int SellOffers { get; set; }

        [Column("buyorders")]
        public int BuyOrders { get; set; }

        [Column("datetime")]
        public DateTime DateTime { get; set; }

        [Column("removed")]
        public bool Removed { get; set; }

        [Column("popularity")]
        public int Popularity { get; set; }

        [Column("meta")]
        public bool Meta { get; set; }

        [Column("workbenchrarity")]
        public int WorkbenchRarity { get; set; }

        [Column("craftingsellsum")]
        public int CraftingSellSum { get; set; }

        [Column("craftingbuysum")]
        public int CraftingBuySum { get; set; }

        [Column("amount")]
        public int Amount { get; set; }

        [ResultColumn]
        [ComplexMapping]
        public ItemLocalizationPoco ItemLocalization { get; set; }

        [ResultColumn]
        [ComplexMapping]
        public RarityPoco Rarity { get; set; }

        [Ignore]
        public List<SalvageRewardPoco> SalvageRewards { get; set; } = new List<SalvageRewardPoco>();

        [Ignore]
        public string Image { get => Id + ".png"; }

        [Ignore]
        public bool ImageExists { get; set; }

        [Ignore]
        public string AvailableName { get => ItemLocalization?.LocalizedName ?? Name; }

        [Ignore]
        public decimal SalvageSellPrice { get => CalculateSalvagePrice(true); }

        [Ignore]
        public decimal SalvageBuyPrice { get => CalculateSalvagePrice(false); }

        [Ignore]
        public decimal SalvageMargin { get => (decimal)(SalvageSellPrice - BuyPrice - (SalvageSellPrice * 0.1m)); }

        [Ignore]
        public string FormatSalvageMargin
        {
            get { return PriceFormatter.FormatPrice(SalvageMargin); }
        }

        [Ignore]
        public string FormatSellPrice
        {
            get { return PriceFormatter.FormatPrice(SellPrice); }
        }

        [Ignore]
        public string FormatBuyPrice
        {
            get { return PriceFormatter.FormatPrice(BuyPrice); }
        }

        [Ignore]
        public string FormatSalvageSellPrice
        {
            get { return PriceFormatter.FormatPrice(SalvageSellPrice); }
        }

        [Ignore]
        public string FormatSalvageBuyPrice
        {
            get { return PriceFormatter.FormatPrice(SalvageBuyPrice); }
        }

        public decimal CalculateSalvageMargin(PriceType salvageItemPrice, PriceType rewardItemPrice)
        {
            decimal buyPrice;
            if (salvageItemPrice == PriceType.Sell)
            {
                buyPrice = SellPrice;
            }
            else
            {
                buyPrice = BuyPrice;
            }

            decimal sellPrice;
            if (rewardItemPrice == PriceType.Sell)
            {
                sellPrice = SalvageSellPrice;
            }
            else
            {
                sellPrice = SalvageBuyPrice;
            }

            return (decimal)(sellPrice - buyPrice - (sellPrice * 0.1m));
        }

        public string FormatCalculateSalvageMargin(PriceType salvageItemPrice, PriceType rewardItemPrice)
        {
            return PriceFormatter.FormatPrice(CalculateSalvageMargin(salvageItemPrice, rewardItemPrice));
        }

        private decimal CalculateSalvagePrice(bool sellPrice)
        {
            decimal price = 0;
            foreach (var salvageReward in SalvageRewards)
            {

                if (sellPrice)
                {
                    price += salvageReward.Item.SellPrice * (salvageReward.RewardAmount / (decimal)salvageReward.Item.Amount);
                }
                else
                {
                    price += salvageReward.Item.BuyPrice * (salvageReward.RewardAmount / (decimal)salvageReward.Item.Amount);
                }
            }
            return price;
        }
    }
}
