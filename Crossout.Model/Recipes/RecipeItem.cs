using System;
using System.Collections.Generic;
using Crossout.Model.Formatter;
using Crossout.Model.Items;

namespace Crossout.Model.Recipes
{
    public class RecipeItem
    {
        public int Id { get; set; }

        public int Depth { get; set; } = 0;

        public Item Item { get; set; }
        public int Number { get; set; }
        
        public decimal SumBuy { get; set; }
        public decimal SumSell { get; set; }

        public string SumBuyFormat => PriceFormatter.FormatPrice(SumBuy);
        public string SumSellFormat => PriceFormatter.FormatPrice(SumSell);

        public decimal BuyPriceTimesNumber => CalculatePriceByNumber(Item.BuyPrice, Number, Item.Id);
        public decimal SellPriceTimesNumber => CalculatePriceByNumber(Item.SellPrice, Number, Item.Id);

        public string FormatBuyPriceTimesNumber
        {
            get
            {
                return PriceFormatter.FormatPrice(BuyPriceTimesNumber);
            }
        }

        public string FormatSellPriceTimesNumber
        {
            get
            {
                return PriceFormatter.FormatPrice(SellPriceTimesNumber);
            }
        }

        private static decimal CalculatePriceByNumber(int price,int number, int id)
        {
            if (id == 43 || id == 53 || id == 85 || id == 168) // Kupfer x100, Scrap x100, Wires x100, Electronics x100
            {
                return price * number / 100m;
            }
            return price * number;
        }

        public RecipeItem Parent { get; set; }

        public List<RecipeItem> Ingredients { get; set; } = new List<RecipeItem>();

        public int ParentId
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.Item.Id;
                }
                return 0;
            }
        }

        public int ParentRecipe
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.Item.RecipeId;
                }
                return 0;
            }
        }

        public int SuperParentRecipe
        {
            get
            {
                if (Parent != null && Parent.Parent != null)
                {
                    return Parent.Parent.Item.RecipeId;
                }
                return 0;
            }
        }

        public static List<RecipeItem> Create(RecipeItem item, List<object[]> dataSet)
        {
            List<RecipeItem> items = new List<RecipeItem>();
            foreach (var row in dataSet)
            {
                var recItem = Create(row);
                recItem.Parent = item;
                items.Add(recItem);
            }

            return items;
        }

        public static RecipeItem Create(object[] row)
        {
            int i = 0;
            RecipeItem recipeItem = new RecipeItem();
            Item item = new Item
            {
                Id = Convert.ToInt32(row[i++]),
                Name = Convert.ToString(row[i++]),
                SellPrice = Convert.ToInt32(row[i++]),
                BuyPrice = Convert.ToInt32(row[i++]),
                SellOffers = Convert.ToInt32(row[i++]),
                BuyOrders = Convert.ToInt32(row[i++]),
                Timestamp = Convert.ToDateTime(row[i++]),
                RarityId = Convert.ToInt32(row[i++]),
                RarityName = Convert.ToString(row[i++]),
                CategoryId = Convert.ToInt32(row[i++]),
                CategoryName = Convert.ToString(row[i++]),
                TypeId = Convert.ToInt32(row[i++]),
                TypeName = Convert.ToString(row[i++])
            };

            if (DBNull.Value == row[i])
            {
                item.RecipeId = 0;
            }
            else
            {
                item.RecipeId = Convert.ToInt32(row[i]);
            }
            i++;
            recipeItem.Number = Convert.ToInt32(row[i++]);
            
            recipeItem.Id = Convert.ToInt32(row[i]);
            recipeItem.Item = item;
            return recipeItem;
        }

        public override string ToString()
        {
            return $"{nameof(Depth)}: {Depth}, {nameof(Item)}: {Item}, {nameof(Number)}: {Number}";
        }
    }
}
