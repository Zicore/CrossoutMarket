using System;
using System.Collections.Generic;
using Crossout.Model.Items;

namespace Crossout.Model.Recipes
{
    public class RecipeItem
    {
        public int Depth { get; set; } = 0;

        public Item Item { get; set; }
        public int Number { get; set; }

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
            recipeItem.Number = Convert.ToInt32(row[i]);
            recipeItem.Item = item;
            return recipeItem;
        }

        public override string ToString()
        {
            return $"{nameof(Depth)}: {Depth}, {nameof(Item)}: {Item}, {nameof(Number)}: {Number}";
        }
    }
}
