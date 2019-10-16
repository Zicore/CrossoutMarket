using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.Model.Items;
using Crossout.Web;
using Crossout.Web.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.AspWeb.Controllers
{
    public class ItemController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public ItemController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("item/{id}")]
        public IActionResult Item(int id)
        {
            return RouteItem(id);
        }

        [Route("data/recipe/{id:int}")]
        public IActionResult Recipe(int id)
        {
            return RouteRecipeData(id);
        }

        private IActionResult RouteItem(int id)
        {
            try
            {
                //RecipeItem.ResetId();
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemModel = db.SelectItem(id, true);
                itemModel.Item.SetImageExists(pathProvider);
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

                return View("item", itemModel);
            }
            catch
            {
                return Redirect("/");
            }
        }

        private IActionResult RouteRecipeData(int id)
        {
            try
            {
                //RecipeItem.ResetId();
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemModel = db.SelectItem(id, false);
                var recipeModel = db.SelectRecipeModel(itemModel.Item, true);

                itemModel.Recipe = recipeModel;

                return Json(itemModel);
            }
            catch
            {
                return Redirect("/");
            }
        }
    }
}