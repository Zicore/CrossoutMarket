using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Model.Recipes;
using Crossout.Web.Models;
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
                var recipeModel = db.SelectRecipeModel(itemModel.Item);
                var statusModel = db.SelectStatus();

                itemModel.Recipe = recipeModel;
                itemModel.Status = statusModel;

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
                var recipeModel = db.SelectRecipeModel(itemModel.Item);

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
