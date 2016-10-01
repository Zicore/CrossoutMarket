using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Model.Recipes;
using Crossout.Web.Models;
using Crossout.Web.Modules.Search;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Services
{
    public class DataService
    {
        protected SqlConnector DB { get; set; }

        public DataService(SqlConnector sql)
        {
            DB = sql;
        }

        public ItemModel SelectItem(int id)
        {
            ItemModel itemModel = new ItemModel();
            var parmeter = new List<Parameter>();
            parmeter.Add(new Parameter { Identifier = "id", Value = id });

            string query = BuildSearchQuery(false, false, false, true, false, false);

            var ds = DB.SelectDataSet(query, parmeter);
            
            var item = Item.Create(ds[0]);
            itemModel.Item = item;
            return itemModel;   
        }

        public RecipeModel SelectRecipeModel(Item item)
        {
            RecipeModel recipeModel = new RecipeModel();
            recipeModel.Recipe = new RecipeItem {Item = item,Ingredients = SelectRecipe(item) };
            
            ResolveRecipe(recipeModel.Recipe, 1);

            CalculateRecipe(recipeModel.Recipe);
            recipeModel.Recipe.IngredientSum = CreateIngredientItem(recipeModel.Recipe);
            

            return recipeModel;
        }

        public void ResolveRecipe(RecipeItem parent, int depth)
        {
            foreach (var ingredient in parent.Ingredients)
            {
                ingredient.Parent = parent;
                ingredient.Depth = depth;
                if (ingredient.Item.RecipeId > 0)
                {
                    ingredient.Ingredients = SelectRecipe(ingredient.Item);
                    ++depth;
                    ResolveRecipe(ingredient, depth);
                    CalculateRecipe(ingredient);
                    if (ingredient.Depth > 0)
                    {
                        ingredient.IngredientSum = CreateIngredientItem(ingredient);
                    }
                    parent.MaxDepth = Math.Max(depth, ingredient.MaxDepth);
                    depth--;
                }               
            }
        }

        private static RecipeItem CreateIngredientItem(RecipeItem item)
        {
            var ingredientSum = new RecipeItem
            {
                Id = -1,
                Depth = item.Depth,
                Item = new Item
                {
                    Id = item.Item.Id,
                    RecipeId = item.Item.RecipeId,

                    Name = item.Item.Name,
                    SellPrice = item.SumSell,
                    BuyPrice = item.SumBuy,
                    SellOffers = item.Item.SellOffers,
                    BuyOrders = item.Item.BuyOrders,
                    RarityId = item.Item.RarityId,
                    RarityName = item.Item.RarityName,
                    CategoryId = item.Item.CategoryId,
                    CategoryName = item.Item.CategoryName,
                    TypeId = item.Item.TypeId,
                    TypeName = item.Item.TypeName
                }
            };
            ingredientSum.Parent = item;
            ingredientSum.IsSumRow = true;
            return ingredientSum;       
        }

        private static void CalculateRecipe(RecipeItem item)
        {
            item.SumBuy = item.Ingredients.Sum(x => x.BuyPriceTimesNumber);
            item.SumSell = item.Ingredients.Sum(x => x.SellPriceTimesNumber);
        }

        public List<RecipeItem> SelectRecipe(Item item)
        {
            var parmeter = new List<Parameter>();
            parmeter.Add(new Parameter { Identifier = "id", Value = item.RecipeId });
            string query = BuildRecipeQuery();
            var ds = DB.SelectDataSet(query, parmeter);
            return RecipeItem.Create(new RecipeItem {Item = item}, ds);
        }

        public static string BuildRecipeQuery()
        {
            string selectColumns = "item.id,item.name,item.sellprice,item.buyprice,item.selloffers,item.buyorders,item.datetime,rarity.id,rarity.name,category.id,category.name,type.id,type.name,recipe2.id,recipeitem.number,recipeitem.id";
            string query =
                $"SELECT {selectColumns} " +
                "FROM recipe " +
                "LEFT JOIN recipeitem ON recipeitem.recipenumber = recipe.id " +
                "LEFT JOIN item ON item.id = recipeitem.itemnumber " +
                "LEFT JOIN rarity ON rarity.id = item.raritynumber " +
                "LEFT JOIN category ON category.id = item.categorynumber " +
                "LEFT JOIN type ON type.id = item.typenumber " +
                "LEFT JOIN recipe recipe2 ON recipe2.itemnumber = recipeitem.itemnumber " +
                "WHERE recipe.id = @id";

            return query;
        }

        public static string BuildSearchQuery(bool hasFilter, bool limit, bool count, bool hasId, bool hasRarity, bool hasCategory)
        {
            string selectColumns = "item.id,item.name,item.sellprice,item.buyprice,item.selloffers,item.buyorders,item.datetime,rarity.id,rarity.name,category.id,category.name,type.id,type.name,recipe.id";
            if (count)
            {
                selectColumns = "count(*)";
            }
            string query = $"SELECT {selectColumns} FROM item LEFT JOIN rarity on rarity.id = item.raritynumber LEFT JOIN category on category.id = item.categorynumber LEFT JOIN type on type.id = item.typenumber LEFT JOIN recipe ON recipe.itemnumber = item.id ";

            if (!hasId)
            {
                if (hasFilter)
                {
                    query += "WHERE item.name LIKE @filter ";
                }
                else
                {
                    query += "WHERE 1=1 ";
                }
            }
            else
            {
                query += "WHERE item.id = @id ";
            }

            if (hasRarity)
            {
                query += " AND rarity.id = @rarity ";
            }

            if (hasCategory)
            {
                query += " AND category.id = @category ";
            }

            if (!count)
            {
                query += "ORDER BY item.id asc, item.name asc ";
            }

            if (limit)
            {
                query += "LIMIT @from,@to";
            }

            return query;
        }
    }
}
