using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model;
using Crossout.Model.Items;
using Crossout.Model.Recipes;
using Crossout.AspWeb.Models;
using Crossout.AspWeb.Models.EditRecipe;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Models.Items;
using Crossout.AspWeb.Models.Recipes;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.Data.PremiumPackages;
using Crossout.AspWeb.Models.Changes;

namespace Crossout.AspWeb.Services
{
    public class DataService
    {
        protected SqlConnector DB { get; set; }

        public DataService(SqlConnector sql)
        {
            DB = sql;
        }

        public ItemModel SelectItem(int id, bool addData)
        {
            ItemModel itemModel = new ItemModel();
            var parmeter = new List<Parameter>();
            parmeter.Add(new Parameter { Identifier = "id", Value = id });

            string query = BuildSearchQuery(false, false, false, true, false, false, false, true, true);

            var ds = DB.SelectDataSet(query, parmeter);
            
            var item = Item.Create(ds[0]);
            if (addData)
            {
                CrossoutDataService.Instance.AddData(item);
            }
            itemModel.Item = item;
            return itemModel;   
        }

        public Dictionary<int, Item> SelectListOfItems(List<int> ids)
        {
            Dictionary<int, Item> items = new Dictionary<int, Item>();
            string query = BuildItemsQueryFromIDList(ids);
            var ds = DB.SelectDataSet(query);

            foreach(var row in ds)
            {
                Item item = new Item();
                int i = 0;
                item.Id = row[i++].ConvertTo<int>();
                item.Name = row[i++].ConvertTo<string>();
                item.SellPrice = row[i++].ConvertTo<int>();
                item.BuyPrice = row[i++].ConvertTo<int>();
                item.Amount = row[i++].ConvertTo<int>();
                items.Add(item.Id, item);
            }

            return items;
        }

        public RecipeModel SelectRecipeModel(Item item, bool resolveDeep, bool addWorkbenchItem = true)
        {
            RecipeModel recipeModel = new RecipeModel();
            RecipeCounter counter = new RecipeCounter();
            recipeModel.Recipe = new RecipeItem(counter) {Item = item,Ingredients = SelectRecipe(counter,item) };
            
            ResolveRecipe(counter, recipeModel.Recipe, 1, resolveDeep, addWorkbenchItem);

            CalculateRecipe(recipeModel.Recipe);
            recipeModel.Recipe.IngredientSum = CreateIngredientItem(counter,recipeModel.Recipe);
            
            return recipeModel;
        }

        public void ResolveRecipe(RecipeCounter counter,RecipeItem parent, int depth, bool resolveDeep, bool addWorkbenchItem)
        {
            foreach (var ingredient in parent.Ingredients)
            {
                ingredient.Parent = parent;
                ingredient.Depth = depth;
                if (ingredient.Item.RecipeId > 0 && resolveDeep)
                {
                    ingredient.Ingredients = SelectRecipe(counter, ingredient.Item);
                    ++depth;
                    ResolveRecipe(counter, ingredient, depth, true, addWorkbenchItem);
                    CalculateRecipe(ingredient);
                    if (ingredient.Depth > 0)
                    {
                        ingredient.IngredientSum = CreateIngredientItem(counter, ingredient);
                    }
                    parent.MaxDepth = Math.Max(depth, ingredient.MaxDepth);
                    depth--;
                }
            }

            if (addWorkbenchItem)
            {
                AddWorkbenchCostItem(counter, parent, depth);
            }
        }

        public WorkbenchItemId GetWorkbenchItemIdByRarity(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common_1:
                    return WorkbenchItemId.Common_445;
                case Rarity.Rare_2:
                    return WorkbenchItemId.Rare_446;
                case Rarity.Epic_3:
                    return WorkbenchItemId.Epic_447;
                case Rarity.Legendary_4:
                    return WorkbenchItemId.Legendary_448;
                case Rarity.Relic_5:
                    return WorkbenchItemId.Relic_449;
                case Rarity.Skins_6:
                    return WorkbenchItemId.Skins_466;
                default: return WorkbenchItemId.Common_445;
            }
        }

        public void AddWorkbenchCostItem(RecipeCounter counter, RecipeItem parent, int depth)
        {
            if (parent.Ingredients.Count > 0)
            {
                var rarity = (Rarity) parent.Item.RarityId;
                // We don't want common (no workbench costs) to be displayed. 
                // Also incase the workbench costs are not based on the rarity we use the override value from the DB
                if (parent.Item.WorkbenchRarity > 0)
                {
                    rarity = (Rarity) parent.Item.WorkbenchRarity;
                }

                if (rarity != Rarity.Common_1)
                {
                    var id = GetWorkbenchItemIdByRarity(rarity);
                    var workbenchItem = SelectItem((int) id, false);
                    parent.Ingredients.Add(CreateIngredientWorkbenchItem(counter, parent, workbenchItem, depth));
                }
            }
        }
        
        private static RecipeItem CreateIngredientItem(RecipeCounter counter,RecipeItem item)
        {
            var ingredientSum = new RecipeItem(counter)
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

        private static RecipeItem CreateIngredientWorkbenchItem(RecipeCounter counter, RecipeItem parent, ItemModel item, int depth)
        {
            var ingredient = new RecipeItem(counter)
            {
                Id = -1,
                Depth = depth,
                Item = new Item
                {
                    Id = item.Item.Id,
                    RecipeId = item.Item.RecipeId,

                    Name = item.Item.Name,
                    SellPrice = item.Item.SellPrice,
                    BuyPrice = item.Item.BuyPrice,
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
            ingredient.Number = 1;
            ingredient.Parent = parent;
            return ingredient;
        }

        private static void CalculateRecipe(RecipeItem item)
        {
            item.SumBuy = item.Ingredients.Sum(x => x.BuyPriceTimesNumber);
            item.SumSell = item.Ingredients.Sum(x => x.SellPriceTimesNumber);
        }

        public List<RecipeItem> SelectRecipe(RecipeCounter counter,Item item)
        {
            var parmeter = new List<Parameter>();
            parmeter.Add(new Parameter { Identifier = "id", Value = item.RecipeId });
            string query = BuildRecipeQuery();
            var ds = DB.SelectDataSet(query, parmeter);
            return RecipeItem.Create(counter, new RecipeItem(counter) {Item = item}, ds);
        }

        public IngredientUsageModel SelectIngredientUsage(int itemId)
        {
            var parmeter = new List<Parameter>();
            parmeter.Add(new Parameter { Identifier = "itemnumber", Value = itemId });
            string query = BuildIngredientUsageQuery();
            var ds = DB.SelectDataSet(query, parmeter);
            var ingredientUsageModel = new IngredientUsageModel();
            foreach (var row in ds)
            {
                if (row.All(x => x != null))
                {
                    int i = 0;
                    var item = new IngredientUsageItem
                    {
                        RecipeId = row[i++].ConvertTo<int>(),
                        ItemId = row[i++].ConvertTo<int>(),
                        Amount = row[i++].ConvertTo<int>()
                    };
                    if (item.ItemId != 0)
                    {
                        ingredientUsageModel.IngredientUsageList.Add(item);
                    }
                }
            }
            return ingredientUsageModel;
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

        public List<Item> SelectAllActiveItems(bool excludeRemovedItems = true)
        {
            return Item.CreateAllItemsForEdit(DB.SelectDataSet(BuildAllActiveItemsQuery(excludeRemovedItems)));
        }

        public List<FactionModel> SelectAllFactions()
        {
            return CreateAllFactionsForEdit(DB.SelectDataSet(BuildFactionsQuery()));
        }

        public Dictionary<int, string> SelectAllRarities()
        {
            var dict = new Dictionary<int, string>();
            var ds = DB.SelectDataSet(BuildRarityQuery());
            foreach (var row in ds)
            {
                dict.Add(row[0].ConvertTo<int>(), row[1].ConvertTo<string>());
            }
            return dict;
        }

        public Dictionary<int, string> SelectAllCategories()
        {
            var dict = new Dictionary<int, string>();
            var ds = DB.SelectDataSet(BuildCategoryQuery());
            foreach (var row in ds)
            {
                dict.Add(row[0].ConvertTo<int>(), row[1].ConvertTo<string>());
            }
            return dict;
        }

        public Dictionary<int, string> SelectAllTypes()
        {
            var dict = new Dictionary<int, string>();
            var ds = DB.SelectDataSet(BuildTypeQuery());
            foreach (var row in ds)
            {
                dict.Add(row[0].ConvertTo<int>(), row[1].ConvertTo<string>());
            }
            return dict;
        }

        public List<AppPrices> SelectAllSteamPrices()
        {
            List<AppPrices> appPrices = new List<AppPrices>();
            var ds = DB.SelectDataSet(BuildSteamPricesQuery());
            foreach (var row in ds)
            {
                List<Currency> currencys = new List<Currency>();
                currencys.Add(new Currency() { Final = row[1].ConvertTo<int>(), CurrencyAbbriviation = "USD" });
                currencys.Add(new Currency() { Final = row[2].ConvertTo<int>(), CurrencyAbbriviation = "EUR" });
                currencys.Add(new Currency() { Final = row[3].ConvertTo<int>(), CurrencyAbbriviation = "GBP" });
                currencys.Add(new Currency() { Final = row[4].ConvertTo<int>(), CurrencyAbbriviation = "RUB" });
                AppPrices appPrice = new AppPrices() { Id = (int)row[0], Prices = currencys, Discount = row[5].ConvertTo<int>(), SuccessTimestamp = row[6].ConvertTo<DateTime>() };
                appPrices.Add(appPrice);
            }
            return appPrices;
        }

        public ChangesModel SelectChanges(int itemId = 0)
        {
            string query = BuildChangesQuery(itemId);
            var ds = DB.SelectDataSet(query);
            var changesModel = new ChangesModel();
            foreach (var row in ds)
            {
                int i = 0;
                var changeItem = new ChangeItem
                {
                    Id = row[i++].ConvertTo<int>(),
                    ItemId = row[i++].ConvertTo<int>(),
                    ChangeType = row[i++].ConvertTo<string>(),
                    Field = (row[i++].ConvertTo<string>()),
                    OldValue = row[i++].ConvertTo<string>(),
                    NewValue = row[i++].ConvertTo<string>(),
                    Timestamp = row[i].ConvertTo<DateTime>()
                };
                changeItem.TranslatedField = TranslateFieldName(changeItem.Field);
                changesModel.Changes.Add(changeItem);
            }
            return changesModel;
        }

        public string TranslateFieldName(string toTranslate)
        {
            switch (toTranslate)
            {
                case "name":
                    return "Name";
                case "rarity":
                    return "Rarity";
                case "category":
                    return "Category";
                case "type":
                    return "Type";
                case "removed":
                    return "Removed Flag";
                case "recipe":
                    return "Recipe";
                case "ingredient":
                    return "Ingredient";
                case "item":
                    return "Item";
                default:
                    return toTranslate;
            }
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
            if (editModelSave.RecipeNumber == 0 && items.Any(x => x.Id > 0))
            {
                var result = DB.Insert("recipe", new string[] {"itemnumber", "factionnumber"}, new object[] { editModelSave.ItemNumber, editModelSave.FactionNumber });
                editModelSave.RecipeNumber = (int)result.LastInsertedId;
                RecordChange(editModelSave.ItemNumber, "ADD", "recipe");
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

                        if (item.OldId != item.Id)
                        {
                            RecordChange(editModelSave.ItemNumber, "UPDATE", "ingredient", item.OldId.ToString(), item.Id.ToString());
                        }

                    }
                    else
                    {
                        // New Item Id is 0 (or below) we delete this item
                        List<Parameter> parameters = new List<Parameter>();
                        parameters.Add(new Parameter
                        {
                            Identifier = "@recipenumber",
                            Value = editModelSave.RecipeNumber
                        });
                        parameters.Add(new Parameter {Identifier = "@recipeitemnumber", Value = item.RecipeItemNumber});
                        var result =
                            DB.ExecuteSQL(
                                "DELETE FROM recipeitem WHERE recipeitem.recipenumber = @recipenumber AND recipeitem.id = @recipeitemnumber;",
                                parameters);
                        RecordChange(editModelSave.ItemNumber, "DELETE", "ingredient", item.OldId.ToString());

                        if (!items.Any(x => x.Id != 0))
                        {
                            List<Parameter> parameters2 = new List<Parameter>();
                            parameters2.Add(new Parameter
                            {
                                Identifier = "@recipenumber",
                                Value = editModelSave.RecipeNumber
                            });
                            var result2 =
                                DB.ExecuteSQL(
                                    "DELETE FROM recipe WHERE recipe.id = @recipenumber;",
                                    parameters);
                            RecordChange(editModelSave.ItemNumber, "DELETE", "recipe");
                        }
                    }
                }
                if (item.OldId == 0)
                {
                    if (item.Id > 0 && item.Number > 0)
                    {
                        DB.Insert("recipeitem", new string[] {"recipenumber", "itemnumber", "number"},
                            new object[] {editModelSave.RecipeNumber, item.Id, item.Number});
                        RecordChange(editModelSave.ItemNumber, "ADD", "ingredient", "", item.Id.ToString());
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

        public void SaveGeneralItemInfo(EditGeneralInfo info, EditModelSave editModelSave)
        {
            var item = SelectItem(editModelSave.ItemNumber, false);

            if (item.Item.Name != info.NewItemName)
            {
                RecordChange(item.Item.Id, "UPDATE", "name", item.Item.Name, info.NewItemName);
            }

            if (item.Item.RarityId != info.NewRarity)
            {
                RecordChange(item.Item.Id, "UPDATE", "rarity", item.Item.RarityId.ToString(), info.NewRarity.ToString());
            }

            if (item.Item.CategoryId != info.NewCategory)
            {
                RecordChange(item.Item.Id, "UPDATE", "category", item.Item.CategoryId.ToString(), info.NewCategory.ToString());
            }

            if (item.Item.TypeId != info.NewType)
            {
                RecordChange(item.Item.Id, "UPDATE", "type", item.Item.TypeId.ToString(), info.NewType.ToString());
            }

            if (item.Item.Removed != Convert.ToInt32(info.NewRemovedStatus))
            {
                RecordChange(item.Item.Id, "UPDATE", "removed", Convert.ToString(item.Item.Removed), Convert.ToString(Convert.ToInt32(info.NewRemovedStatus)));
            }

            int removed = 0;
            if (info.NewRemovedStatus)
                removed = 1;
            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter { Identifier = "@id", Value = editModelSave.ItemNumber });
            parameters.Add(new Parameter { Identifier = "@locname", Value = info.NewItemName });
            parameters.Add(new Parameter { Identifier = "@rarity", Value = info.NewRarity });
            parameters.Add(new Parameter { Identifier = "@category", Value = info.NewCategory });
            parameters.Add(new Parameter { Identifier = "@type", Value = info.NewType });
            parameters.Add(new Parameter { Identifier = "@removed", Value = removed });
            var result = DB.ExecuteSQL("UPDATE item SET item.name = @locname, item.raritynumber = @rarity, item.categorynumber = @category, item.typenumber = @type, item.removed = @removed WHERE item.id = @id", parameters);
        }

        public void RecordChange(int itemId, string type, string field, string oldValue = "", string newValue = "")
        {
            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter { Identifier = "@itemid", Value = itemId });
            parameters.Add(new Parameter { Identifier = "@type", Value = type });
            parameters.Add(new Parameter { Identifier = "@field", Value = field });
            parameters.Add(new Parameter { Identifier = "@oldValue", Value = oldValue });
            parameters.Add(new Parameter { Identifier = "@newValue", Value = newValue });
            parameters.Add(new Parameter { Identifier = "@datetime", Value = DateTime.UtcNow });
            var result = DB.ExecuteSQL("INSERT INTO changes SET changes.itemid = @itemid, changes.changetype = @type, changes.field = @field,  changes.oldValue = @oldValue,  changes.newValue = @newValue, changes.datetime = @datetime", parameters);
        }

        public static string BuildStatusQuery()
        {
            string query = "SELECT item.id,item.datetime as datetime FROM item ORDER BY item.datetime DESC LIMIT 1;";
            return query;
        }

        public static string BuildRecipeQuery()
        {
            string selectColumns = "item.id,item.name,item.sellprice,item.buyprice,item.selloffers,item.buyorders,item.datetime,rarity.id,rarity.name,category.id,category.name,type.id,type.name,recipe2.id,recipeitem.number,recipeitem.id,recipe.factionnumber,faction.name";
            string query =
                $"SELECT {selectColumns} " +
                "FROM recipe " +
                "LEFT JOIN recipeitem ON recipeitem.recipenumber = recipe.id " +
                "LEFT JOIN item ON item.id = recipeitem.itemnumber " +
                "LEFT JOIN rarity ON rarity.id = item.raritynumber " +
                "LEFT JOIN category ON category.id = item.categorynumber " +
                "LEFT JOIN type ON type.id = item.typenumber " +
                "LEFT JOIN recipe recipe2 ON recipe2.itemnumber = recipeitem.itemnumber " +
                "LEFT JOIN faction faction ON faction.id = recipe.factionnumber " +
                "WHERE recipe.id = @id";

            return query;
        }

        public static string BuildIngredientUsageQuery()
        {
            string collumns = "recipe.id, recipe.itemnumber, recipeitem.number";
            string tables = "crossout.recipeitem LEFT JOIN recipe ON recipeitem.recipenumber = recipe.id";
            string query = $"SELECT {collumns} FROM {tables} WHERE recipeitem.itemnumber = @itemnumber";
            return query;
        }

        public static string BuildSearchQuery(bool hasFilter, bool limit, bool count, bool hasId, bool hasRarity, bool hasCategory, bool hasFaction, bool showRemovedItems, bool showMetaItems)
        {
            string selectColumns = "item.id,item.name,item.sellprice,item.buyprice,item.selloffers,item.buyorders,item.datetime,rarity.id,rarity.name,category.id,category.name,type.id,type.name,recipe.id,item.removed,item.meta,faction.id,faction.name,item.popularity,item.workbenchrarity,item.craftingsellsum,item.craftingbuysum,item.amount";
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

            if (!showMetaItems)
            {
                query += " AND item.meta = 0 ";
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

        public static string BuildAllActiveItemsQuery(bool excludeRemovedItems = true)
        {
            string removedItems = "";
            if (excludeRemovedItems)
            {
                removedItems = "WHERE removed = 0";
            }
            string query = $"SELECT item.id,item.name FROM item {removedItems} ORDER BY item.name ASC,item.id ASC;";
            return query;
        }

        public static string BuildFactionsQuery()
        {
            string query = "SELECT faction.id, faction.name FROM faction ORDER BY id ASC;";
            return query;
        }

        public static string BuildRarityQuery()
        {
            string query = "SELECT rarity.id, rarity.name FROM rarity ORDER BY id ASC;";
            return query;
        }

        public static string BuildCategoryQuery()
        {
            string query = "SELECT category.id, category.name FROM category ORDER BY id ASC;";
            return query;
        }

        public static string BuildTypeQuery()
        {
            string query = "SELECT type.id, type.name FROM type ORDER BY id ASC;";
            return query;
        }

        public static string BuildItemsQueryFromIDList(List<int> ids)
        {
            StringBuilder sb = new StringBuilder();
            string query = "SELECT item.id, item.name, item.sellprice, item.buyprice, item.amount FROM item WHERE ";
            sb.Append(query);
            int i = 0;
            foreach(var id in ids)
            {
                if (i == 0)
                {
                    sb.Append("id=");
                    sb.Append(id);
                }
                else
                {
                    sb.Append(" OR id=");
                    sb.Append(id);
                }
                i++;
            }
            query = sb.ToString();
            return query;
        }

        public static string BuildSteamPricesQuery()
        {
            string collumns = "steamprices.appid,steamprices.priceusd,steamprices.priceeur,steamprices.pricegbp,steamprices.pricerub,steamprices.discount,steamprices.successtimestamp";
            string query = $"SELECT {collumns} FROM steamprices";
            return query;
        }

        public static string BuildCraftingOverviewQuery()
        {
            string collumns = "item.id,item.name,item.sellprice,item.buyprice,item.selloffers,item.buyorders,item.datetime,rarity.id,rarity.name,category.id,category.name,type.id,type.name,recipe.id,item.removed,faction.id,faction.name,item.popularity,item.workbenchrarity,item.craftingsellsum,item.craftingbuysum,item.amount";
            string tables = "item LEFT JOIN rarity on rarity.id = item.raritynumber LEFT JOIN category on category.id = item.categorynumber LEFT JOIN type on type.id = item.typenumber LEFT JOIN recipe ON recipe.itemnumber = item.id LEFT JOIN faction ON faction.id = recipe.factionnumber";
            string query = $"SELECT {collumns} FROM {tables} WHERE removed=0 AND meta=0 AND craftingsellsum!=0 AND craftingbuysum!=0 ORDER BY item.id";
            return query;
        }

        public static string BuildHtmlExport()
        {
            string collumns = "item.id,item.name,item.sellprice,item.buyprice,item.selloffers,item.buyorders,item.datetime,rarity.id,rarity.name,category.id,category.name,type.id,type.name,recipe.id,item.removed,item.meta,faction.id,faction.name,item.popularity,item.workbenchrarity,item.craftingsellsum,item.craftingbuysum,item.amount";
            string tables = "item LEFT JOIN rarity on rarity.id = item.raritynumber LEFT JOIN category on category.id = item.categorynumber LEFT JOIN type on type.id = item.typenumber LEFT JOIN recipe ON recipe.itemnumber = item.id LEFT JOIN faction ON faction.id = recipe.factionnumber";
            string query = $"SELECT {collumns} FROM {tables} WHERE removed=0 AND meta=0 ORDER BY item.id";
            return query;
        }

        public static string BuildChangesQuery(int itemId = 0)
        {
            string collumns = "changes.id,changes.itemid,changes.changetype,changes.field,changes.oldvalue,changes.newvalue,changes.datetime";
            string tables = "changes";
            string query;
            if (itemId != 0)
            {
                query = $"SELECT {collumns} FROM {tables} WHERE changes.itemid = {itemId} ORDER BY changes.id DESC LIMIT 100";
            }
            else
            {
                query = $"SELECT {collumns} FROM {tables} ORDER BY changes.id DESC LIMIT 500";
            }
            
            return query;
        }

        public static string BuildTrendsQuery(DateTime time)
        {
            string collumns = "market.itemnumber, market.sellprice, market.buyprice, market.selloffers, market.buyorders, market.datetime";
            string tables = "market";
            string query = $"SELECT {collumns} FROM {tables} WHERE market.datetime = '{time.ToString("yyyy-MM-dd HH:mmm:ss")}'";
            return query;
        }
    }
}
