using System;
using System.Collections.Generic;
using Crossout.Model.Formatter;
using Crossout.Model.Items;
using Newtonsoft.Json;

namespace Crossout.Model.Recipes
{
    public class RecipeItem
    {
        private RecipeCounter counter;

        public RecipeItem(RecipeCounter counter)
        {
            UniqueId = counter.NextId();
        }

        public RecipeItem()
        {
            
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("uniqueId")]
        public int UniqueId { get; set; }

        [JsonProperty("rootNumber")]
        public int RootNumber
        {
            get
            {
                var number = Number;
                RecipeItem p = Parent;
                while (p != null)
                {
                    number = number * Math.Max(p.Number, 1);
                    p = p.Parent;
                }
                return number;
            }
        }

        [JsonProperty("factionNumber")]
        public int FactionNumber { get; set; }

        [JsonProperty("depth")]
        public int Depth { get; set; } = 0;

        [JsonProperty("maxDepth")]
        public int MaxDepth { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("sumBuy")]
        public decimal SumBuy { get; set; }

        [JsonProperty("sumSell")]
        public decimal SumSell { get; set; }

        [JsonProperty("sumBuyFormat")]
        public string SumBuyFormat => PriceFormatter.FormatPrice(SumBuy);

        [JsonProperty("sumSellFormat")]
        public string SumSellFormat => PriceFormatter.FormatPrice(SumSell);

        [JsonProperty("buyPriceTimesNumber")]
        public decimal BuyPriceTimesNumber => CalculatePriceByNumber(Item.BuyPrice, Number, Item.Id);

        [JsonProperty("sellPriceTimesNumber")]
        public decimal SellPriceTimesNumber => CalculatePriceByNumber(Item.SellPrice, Number, Item.Id);

        [JsonProperty("isSumRow")]
        public bool IsSumRow { get; set; } = false;

        [JsonProperty("formatBuyPriceTimesNumber")]
        public string FormatBuyPriceTimesNumber => PriceFormatter.FormatPrice(BuyPriceTimesNumber);

        [JsonProperty("formatSellPriceTimesNumber")]
        public string FormatSellPriceTimesNumber => PriceFormatter.FormatPrice(SellPriceTimesNumber);

        static readonly HashSet<int> ResourceNumbers = new HashSet<int>()
        {
            43, //Copper x100
            53 , //Scrap x100
            85, //Wires x100
            119, //Coupons x100
            168, //Electronics x100
            330, //Taler x100
            337,  //Uran x100
            522, // Sweets x100
            784, //Batteries x100
            785 //Plastic x100
        };

        private static decimal CalculatePriceByNumber(decimal price, int number, int id)
        {
            if (ResourceNumbers.Contains(id)) 
            {
                return price * number / 100m;
            }
            return price * number;
        }

        [JsonIgnore]
        public RecipeItem Parent { get; set; }

        [JsonProperty("parentId")]
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

        [JsonProperty("parentUniqueId")]
        public int ParentUniqueId
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.UniqueId;
                }
                return 0;
            }
        }

        [JsonProperty("parentRecipe")]
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

        [JsonProperty("superParentRecipe")]
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

        [JsonProperty("ingredientSum")]
        public RecipeItem IngredientSum { get; set; }

        [JsonProperty("item")]
        public Item Item { get; set; } = new Item();

        [JsonProperty("ingredients")]
        public List<RecipeItem> Ingredients { get; set; } = new List<RecipeItem>();
        
        public static List<RecipeItem> Create(RecipeCounter counter, RecipeItem item, List<object[]> dataSet)
        {
            List<RecipeItem> items = new List<RecipeItem>();
            foreach (var row in dataSet)
            {
                var recItem = Create(counter, row);
                recItem.Parent = item;
                items.Add(recItem);
            }

            return items;
        }
        
        public static RecipeItem Create(RecipeCounter counter,object[] row)
        {
            int i = 0;
            RecipeItem recipeItem = new RecipeItem(counter);
            Item item = new Item
            {
                Id = row[i++].ConvertTo<int>(),
                Name = row[i++].ConvertTo<string>(),
                SellPrice = row[i++].ConvertTo<int>(),
                BuyPrice = row[i++].ConvertTo<int>(),
                SellOffers = row[i++].ConvertTo<int>(),
                BuyOrders = row[i++].ConvertTo<int>(),
                Timestamp = row[i++].ConvertTo<DateTime>(),
                RarityId =row[i++].ConvertTo<int>(),
                RarityName = row[i++].ConvertTo<string>(),
                CategoryId = row[i++].ConvertTo<int>(),
                CategoryName = row[i++].ConvertTo<string>(),
                TypeId = row[i++].ConvertTo<int>(),
                TypeName = row[i++].ConvertTo<string>()
            };

            if (DBNull.Value == row[i])
            {
                item.RecipeId = 0;
            }
            else
            {
                item.RecipeId = row[i].ConvertTo<int>();
            }
            i++;
            recipeItem.Number = row[i++].ConvertTo<int>();

            recipeItem.Id = row[i++].ConvertTo<int>();
            recipeItem.FactionNumber = row[i++].ConvertTo<int>();

            item.FactionNumber = recipeItem.FactionNumber;
            item.Faction = row[i++].ConvertTo<string>();

            item.LocalizedName = row[i].ConvertTo<string>();
            recipeItem.Item = item;


            return recipeItem;
        }

        public override string ToString()
        {
            return $"{nameof(Depth)}: {Depth}, {nameof(Item)}: {Item}, {nameof(Number)}: {Number}";
        }
    }
}
