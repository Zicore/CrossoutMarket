using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Crossout.Data;
using Crossout.Model.Formatter;
using Newtonsoft.Json;

namespace Crossout.Model.Items
{
    public class Item
    {
        public PartStats Stats { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int SellOffers { get; set; }

        public decimal SellPrice { get; set; }

        public int BuyOrders { get; set; }

        public decimal BuyPrice { get; set; }

        public int Removed { get; set; }

        public int Popularity { get; set; }

        public decimal Margin
        {
            get { return (decimal)(SellPrice - BuyPrice - (SellPrice * 0.1m)); }
        }

        public string FormatMargin
        {
            get { return PriceFormatter.FormatPrice(Margin); }
        }

        public DateTime Timestamp { get; set; }

        public string LastUpdateTime => Timestamp.ToString("yyyy-MM-dd HH:mm:ss");

        public int RarityId { get; set; }
        public string RarityName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TypeId { get; set; }
        public int RecipeId { get; set; }
        public string TypeName { get; set; }

        public int FactionNumber { get; set; }
        public string Faction { get; set; }

        public bool OlderThan(int minutes)
        {
            return DateTime.Now - Timestamp > new TimeSpan(0, minutes, 0);
        }

        public string FormatBuyPrice
        {
            get
            {
                return PriceFormatter.FormatPrice(BuyPrice);
            }
        }

        public string FormatSellPrice
        {
            get
            {
                return PriceFormatter.FormatPrice(SellPrice);
            }
        }

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
                Popularity = row[i].ConvertTo<int>(),
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
