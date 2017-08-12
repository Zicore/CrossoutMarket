using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Crossout.Data;
using Crossout.Data.Descriptions;
using Crossout.Data.Stats;
using Crossout.Data.Stats.Main;
using Crossout.Model.Formatter;
using Newtonsoft.Json;

namespace Crossout.Model.Items
{
    // Matches with DB Ids

    public enum Rarity
    {
        Common_1 = 1,
        Rare_2 = 2,
        Epic_3 = 3,
        Legendary_4 = 4,
        Relic_5 = 5
    }

    // Matches with DB Ids

    public enum WorkbenchItemId
    {
        //445	Common Minimum Bench Cost
        //446	Rare Minimum Bench Cost
        //447	Epic Minimum Bench Cost
        //448	Legendary Minimum Bench Cost
        //449	Relic Minimum Bench Cost

        Common_445 = 445,
        Rare_446 = 446,
        Epic_447 = 447,
        Legendary_448 = 448,
        Relic_449 = 449,
    }

    public class Item
    {
        [JsonIgnore]
        public PartStatsBase Stats { get; set; }

        [JsonIgnore]
        public List<SingleStat> SortedStats
        {
            get { return Stats.SortedStats; }
        }

        [JsonProperty("description")]
        public ItemDescription Description { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("selloffers")]
        public int SellOffers { get; set; }

        [JsonProperty("sellprice")]
        public decimal SellPrice { get; set; }

        [JsonProperty("buyorders")]
        public int BuyOrders { get; set; }

        [JsonProperty("buyprice")]
        public decimal BuyPrice { get; set; }

        [JsonProperty("removed")]
        public int Removed { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }

        [JsonProperty("workbenchrarity")]
        public int WorkbenchRarity { get; set; }

        [JsonProperty("margin")]
        public decimal Margin
        {
            get { return (decimal)(SellPrice - BuyPrice - (SellPrice * 0.1m)); }
        }

        [JsonProperty("formatmargin")]
        public string FormatMargin
        {
            get { return PriceFormatter.FormatPrice(Margin); }
        }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("lastupdatetime")]
        public string LastUpdateTime => Timestamp.ToString("yyyy-MM-dd HH:mm:ss");

        [JsonProperty("rarityid")]
        public int RarityId { get; set; }

        [JsonProperty("rarityname")]
        public string RarityName { get; set; }

        [JsonProperty("categoryid")]
        public int CategoryId { get; set; }

        [JsonProperty("categoryname")]
        public string CategoryName { get; set; }

        [JsonProperty("typeid")]
        public int TypeId { get; set; }

        [JsonProperty("recipeid")]
        public int RecipeId { get; set; }

        [JsonProperty("typename")]
        public string TypeName { get; set; }

        [JsonProperty("factionnumber")]
        public int FactionNumber { get; set; }

        [JsonProperty("faction")]
        public string Faction { get; set; }

        public bool OlderThan(int minutes)
        {
            return DateTime.Now - Timestamp > new TimeSpan(0, minutes, 0);
        }

        [JsonProperty("formatbuyprice")]
        public string FormatBuyPrice
        {
            get
            {
                return PriceFormatter.FormatPrice(BuyPrice);
            }
        }

        [JsonProperty("formatsellprice")]
        public string FormatSellPrice
        {
            get
            {
                return PriceFormatter.FormatPrice(SellPrice);
            }
        }

        [JsonProperty("image")]
        public string Image
        {
            get { return $"{Id}.png"; }
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(SellOffers)}: {SellOffers}, {nameof(SellPrice)}: {SellPrice}, {nameof(BuyOrders)}: {BuyOrders}, {nameof(BuyPrice)}: {BuyPrice}";
        }

        public static Item Create(object[] row)
        {
            int i = 0;
            Item item = new Item
            {
                Id = row[i++].ConvertTo<int>(),
                Name = row[i++].ConvertTo<string>(),
                SellPrice = row[i++].ConvertTo<decimal>(),
                BuyPrice = row[i++].ConvertTo<decimal>(),
                SellOffers = row[i++].ConvertTo<int>(),
                BuyOrders = row[i++].ConvertTo<int>(),
                Timestamp = row[i++].ConvertTo<DateTime>(),
                RarityId = row[i++].ConvertTo<int>(),
                RarityName = row[i++].ConvertTo<string>(),
                CategoryId = row[i++].ConvertTo<int>(),
                CategoryName = row[i++].ConvertTo<string>(),
                TypeId = row[i++].ConvertTo<int>(),
                TypeName = row[i++].ConvertTo<string>(),
                RecipeId = row[i++].ConvertTo<int>(),
                Removed = row[i++].ConvertTo<int>(),
                FactionNumber = row[i++].ConvertTo<int>(),
                Faction = row[i++].ConvertTo<string>(),
                Popularity = row[i++].ConvertTo<int>(),
                WorkbenchRarity = row[i].ConvertTo<int>(),
            };

            return item;
        }

        public static Item CreateForChart(object[] row)
        {
            int i = 0;
            Item item = new Item
            {
                Id = row[i++].ConvertTo<int>(),
                SellPrice = row[i++].ConvertTo<int>(),
                BuyPrice = row[i++].ConvertTo<int>(),
                SellOffers = row[i++].ConvertTo<int>(),
                BuyOrders = row[i++].ConvertTo<int>(),
                Timestamp = row[i++].ConvertTo<DateTime>(),
            };
            return item;
        }

        public static List<Item> CreateAllItemsForEdit(List<object[]> data)
        {
            List<Item> items = new List<Item>();
            foreach (var row in data)
            {
                Item item = new Item
                {
                    Id = row[0].ConvertTo<int>(),
                    Name = row[1].ConvertTo<string>()
                };
                items.Add(item);
            }
            return items;
        }
    }
}
