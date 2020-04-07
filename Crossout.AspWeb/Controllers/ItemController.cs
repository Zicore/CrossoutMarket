using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.Model.Items;
using Crossout.Model.Recipes;
using Crossout.AspWeb;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.AspWeb.Models.Language;

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
            Language lang = this.ReadLanguageCookie(sql);
            this.RegisterHit("Item", id);
            return RouteItem(id, lang.Id);
        }

        [Route("data/recipe/{id:int}")]
        public IActionResult Recipe(int id, string l)
        {
            Language lang = this.VerifyLanguage(sql, l);
            return RouteRecipeData(id, lang.Id);
        }

        private IActionResult RouteItem(int id, int language)
        {
            try
            {
                //RecipeItem.ResetId();
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                language = Math.Max(language, 1);

                var itemModel = db.SelectItem(id, true, language);
                itemModel.Item.SetImageExists(pathProvider);

                var recipeModel = db.SelectRecipeModel(itemModel.Item, true, language);
                SetImageExistsRecursive(recipeModel.Recipe);

                var statusModel = db.SelectStatus();
                var changesModel = db.SelectChanges(id);
                var ingredientUsageModel = db.SelectIngredientUsage(id);

                var allItems = db.SelectAllActiveItems(language, false);
                var itemDict = new Dictionary<int, Item>();

                foreach (var item in allItems)
                {
                    item.SetImageExists(pathProvider);
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

        private IActionResult RouteRecipeData(int id, int language)
        {
            try
            {
                //RecipeItem.ResetId();
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);
                language = Math.Max(language, 1);
                var itemModel = db.SelectItem(id, false, language);
                var recipeModel = db.SelectRecipeModel(itemModel.Item, true, language);

                itemModel.Recipe = recipeModel;

                return Json(itemModel);
            }
            catch
            {
                return Redirect("/");
            }
        }

        private void SetImageExistsRecursive(RecipeItem item)
        {
            item.Item.SetImageExists(pathProvider);

            if (item.IngredientSum != null)
            {
                item.IngredientSum.Item.SetImageExists(pathProvider);
            }

            if (item.Ingredients != null)
            {
                foreach (var ingredient in item.Ingredients)
                {
                    SetImageExistsRecursive(ingredient);
                }
            }
        }
    }
}