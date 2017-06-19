using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model;
using Crossout.Model.Items;
using Crossout.Model.Recipes;
using Crossout.Web.Models;
using Crossout.Web.Models.EditRecipe;
using Crossout.Web.Models.General;
using Crossout.Web.Models.Items;
using Crossout.Web.Models.Recipes;
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

            string query = BuildSearchQuery(false, false, false, true, false, false, false, true);

            var ds = DB.SelectDataSet(query, parmeter);
            
            var item = Item.Create(ds[0]);
            CrossoutDataService.Instance.AddStats(item);
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

        public StatusModel SelectStatus()
        {
            var ds = DB.SelectDataSet(BuildStatusQuery());
            var model = new StatusModel
            {
                Id = Convert.ToInt32(ds[0][0]),
                LastUpdate = Convert.ToDateTime(ds[0][1])
            };
            
            return model;
        }

        public List<Item> SelectAllActiveItems()
        {
            return Item.CreateAllItemsForEdit(DB.SelectDataSet(BuildAllActiveItemsQuery()));
        }

        public List<FactionModel> SelectAllFactions()
        {
            return CreateAllFactionsForEdit(DB.SelectDataSet(BuildFactionsQuery()));
        }

        public static List<FactionModel> CreateAllFactionsForEdit(List<object[]> data)
        {
            List<FactionModel> items = new List<FactionModel>();
            foreach (var row in data)
            {
                FactionModel item = new FactionModel
                {
                    Id = row[0].ConvertTo<int>(),
                    Name = row[1].ConvertTo<string>()
                };
                items.Add(item);
            }
            return items;
        }

        public void SaveRecipe(EditModelSave editModelSave, List<EditItem> items)
        {
            if (editModelSave.RecipeNumber == 0)
            {
                var result = DB.Insert("recipe", new string[] {"itemnumber", "factionnumber"}, new object[] { editModelSave.ItemNumber, editModelSave.FactionNumber });
                editModelSave.RecipeNumber = (int)result.LastInsertedId;
            }

            foreach (var item in items)
            {
                if (item.OldId > 0)
                {
                    if (item.Id > 0)
                    {
                        // New Item Id is above 0 so we update this item
                        List<Parameter> parameters = new List<Parameter>();
                        parameters.Add(new Parameter
                        {
                            Identifier = "@recipenumberWhere",
                            Value = editModelSave.RecipeNumber
                        });
                        parameters.Add(new Parameter {Identifier = "@recipeitemnumber", Value = item.RecipeItemNumber});

                        var rs = DB.Update("recipeitem",
                            new string[] {"itemnumber", "number"},
                            new object[] {item.Id, item.Number},
                            "recipenumber = @recipenumberWhere AND recipeitem.id = @recipeitemnumber",
                            parameters);
                    }
                    else
                    {
                        // New Item Id is 0 (or below) we delete this item
                        List<Parameter> parameters = new List<Parameter>();
                        parameters.Add(new Parameter
                        {
                            Identifier = "@recipenumberWhere",
                            Value = editModelSave.RecipeNumber
                        });
                        parameters.Add(new Parameter {Identifier = "@recipeitemnumber", Value = item.RecipeItemNumber});
                        var result =
                            DB.ExecuteSQL(
                                "DELETE FROM recipeitem WHERE recipenumber = @recipenumberWhere AND recipeitem.id = @recipeitemnumber;",
                                parameters);
                    }
                }
                if (item.OldId == 0)
                {
                    if (item.Id > 0 && item.Number > 0)
                    {
                        DB.Insert("recipeitem", new string[] {"recipenumber", "itemnumber", "number"},
                            new object[] {editModelSave.RecipeNumber, item.Id, item.Number});
                    }
                }
            }

            if (editModelSave.OldFactionNumber > 0 && editModelSave.FactionNumber > 0 && editModelSave.FactionNumber != editModelSave.OldFactionNumber)
            {
                List<Parameter> parameters = new List<Parameter>();
                parameters.Add(new Parameter { Identifier = "@factionnumber", Value = editModelSave.FactionNumber });
                parameters.Add(new Parameter { Identifier = "@recipenumber", Value = editModelSave.RecipeNumber });
                var result = DB.ExecuteSQL("UPDATE recipe SET recipe.factionnumber = @factionnumber WHERE recipe.id = @recipenumber", parameters);
            }
        }

        public static string BuildStatusQuery()
        {
            string query = "SELECT item.id,item.datetime as datetime FROM item ORDER BY item.datetime DESC LIMIT 1;";
            return query;
        }

        public static string BuildRecipeQuery()
        {
            string selectColumns = "item.id,item.name,item.sellprice,item.buyprice,item.selloffers,item.buyorders,item.datetime,rarity.id,rarity.name,category.id,category.name,type.id,type.name,recipe2.id,recipeitem.number,recipeitem.id,recipe.factionnumber";
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

        public static string BuildSearchQuery(bool hasFilter, bool limit, bool count, bool hasId, bool hasRarity, bool hasCategory, bool hasFaction, bool showRemovedItems)
        {
            string selectColumns = "item.id,item.name,item.sellprice,item.buyprice,item.selloffers,item.buyorders,item.datetime,rarity.id,rarity.name,category.id,category.name,type.id,type.name,recipe.id,item.removed,faction.id,faction.name,item.popularity";
            if (count)
            {
                selectColumns = "count(*)";
            }
            string query = $"SELECT {selectColumns} FROM item LEFT JOIN rarity on rarity.id = item.raritynumber LEFT JOIN category on category.id = item.categorynumber LEFT JOIN type on type.id = item.typenumber LEFT JOIN recipe ON recipe.itemnumber = item.id LEFT JOIN faction ON faction.id = recipe.factionnumber ";

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

            if (hasFaction)
            {
                query += " AND faction.id = @faction ";
            }

            if (!showRemovedItems)
            {
                query += " AND item.removed = 0 ";
            }

            if (!count)
            {
                query += "ORDER BY item.id asc, item.name asc ";
            }

            //if (limit)
            //{
            //    query += "LIMIT @from,@to";
            //}

            return query;
        }

        public static string BuildAllActiveItemsQuery()
        {
            string query = "SELECT item.id,item.name FROM item where removed = 0 ORDER BY item.name ASC,item.id ASC;";
            return query;
        }

        public static string BuildFactionsQuery()
        {
            string query = "SELECT faction.id, faction.name FROM faction ORDER BY id ASC;";
            return query;
        }
    }
}
