using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Model.Recipes;
using Crossout.Web.Models;
using Crossout.Web.Models.Recipes;
using Crossout.Web.Services;
using Nancy;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Modules.Search
{
    public class ItemModule : NancyModule
    {
        public ItemModule()
        {
            Get["/item/{id:int}"] = x =>
            {
                var id = (int)x.id;
                return RouteItem(id);
            };

            Get["data/recipe/{id:int}"] = x =>
            {
                var id = (int)x.id;
                return RouteRecipeData(id);
            };
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private dynamic RouteItem(int id)
        {
            try
            {
                //RecipeItem.ResetId();
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemModel = db.SelectItem(id, true);
                var recipeModel = db.SelectRecipeModel(itemModel.Item, true);
                var statusModel = db.SelectStatus();
                var changesModel = db.SelectChanges(id);
                var ingredientUsageModel = db.SelectIngredientUsage(id);

                var allItems = db.SelectAllActiveItems(false);
                var itemDict = new Dictionary<int, Item>();

                foreach (var item in allItems)
                {
                    itemDict.Add(item.Id, item);
                }
                changesModel.ContainedItems = itemDict;

                if (changesModel.Changes.Count > 0)
                {
                    
                    var allRarites = db.SelectAllRarities();
                    allRarites.Add(0, "None");
                    var allCategories = db.SelectAllCategories();
                    allCategories.Add(0, "None");
                    var allTypes = db.SelectAllTypes();
                    
                    
                    changesModel.AllRarites = allRarites;
                    changesModel.AllCategories = allCategories;
                    changesModel.AllTypes = allTypes;
                }
                itemModel.Recipe = recipeModel;
                itemModel.Status = statusModel;
                itemModel.Changes = changesModel;
                itemModel.IngredientUsage = ingredientUsageModel;

                return View["item", itemModel];
            }
            catch
            {
                return Response.AsRedirect("/");
            }
        }

        private dynamic RouteRecipeData(int id)
        {
            try
            {
                //RecipeItem.ResetId();
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemModel = db.SelectItem(id, false);
                var recipeModel = db.SelectRecipeModel(itemModel.Item, true);

                itemModel.Recipe = recipeModel;
                
                return Response.AsJson(itemModel);
            }
            catch
            {
                return Response.AsRedirect("/");
            }
        }
    }
}
