using System;
using System.Collections.Generic;
using Crossout.Model.Items;
using Crossout.Web.Models.API.v1;
using Crossout.Web.Models.Filter;
using Crossout.Web.Models.General;
using Crossout.Web.Models.Pagination;
using Crossout.Web.Services;
using Crossout.Web.Services.API.v1;
using Nancy;
using Newtonsoft.Json;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Modules.API.v1
{
    public class ApiModule : NancyModule
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        public ApiModule()
        {
            Get["/api/v1/rarities"] = x =>
            {
                sql.Open(WebSettings.Settings.CreateDescription());
                ApiDataService dataService = new ApiDataService(sql);

                var model = dataService.GetRarities();

                return Response.AsJson(model);
            };

            Get["/api/v1/factions"] = x =>
            {
                sql.Open(WebSettings.Settings.CreateDescription());
                ApiDataService dataService = new ApiDataService(sql);

                var model = dataService.GetFactions();

                return Response.AsJson(model);
            };

            Get["/api/v1/types"] = x =>
            {
                sql.Open(WebSettings.Settings.CreateDescription());
                ApiDataService dataService = new ApiDataService(sql);

                var model = dataService.GetItemTypes();

                return Response.AsJson(model);
            };

            Get["/api/v1/categories"] = x =>
            {
                sql.Open(WebSettings.Settings.CreateDescription());
                ApiDataService dataService = new ApiDataService(sql);

                var model = dataService.GetCategories();

                return Response.AsJson(model);
            };

            Get["/api/v1/items"] = x =>
            {
                return RouteSearch(null, 0, null, null, null, null, null, 0);
            };

            Get["/api/v1/items"] = x =>
            {
                string rarity = (string)Request.Query.Rarity;
                string category = (string)Request.Query.Category;
                string faction = (string)Request.Query.Faction;
                string showRemovedItems = (string)Request.Query.RmdItems;
                string showMetaItems = (string)Request.Query.MItems;
                var query = (string)Request.Query.Query;
                int id = (int) Request.Query.Id;
                
                return RouteSearch(query, 0, rarity, category, faction, showRemovedItems, showMetaItems, id);
            };

            Get["/api/v1/item/{item}"] = x =>
            {
                string rarity = (string)Request.Query.Rarity;
                string category = (string)Request.Query.Category;
                string faction = (string)Request.Query.Faction;
                string showRemovedItems = (string)Request.Query.RmdItems;
                string showMetaItems = (string)Request.Query.MItems;
                var query = (string)Request.Query.Query;
                int id = (int)x.item;

                return RouteSearch(query, 0, rarity, category, faction, showRemovedItems, showMetaItems, id);
            };
            
            Get["/api/v1/recipe/{id:int}"] = x =>
            {
                var id = (int)x.id;

                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemModel = db.SelectItem(id, true);
                var recipeModel = db.SelectRecipeModel(itemModel.Item, false);

                //itemModel.Recipe = recipeModel;

                return Response.AsJson(recipeModel);
            };

            Get["/api/v1/recipe-deep/{id:int}"] = x =>
            {
                var id = (int)x.id;
                
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemModel = db.SelectItem(id, true);
                var recipeModel = db.SelectRecipeModel(itemModel.Item, true);

                itemModel.Recipe = recipeModel;

                return Response.AsJson(itemModel);
            };

            //Get["/api/v1/recipe"] = x =>
            //{
            //    sql.Open(WebSettings.Settings.CreateDescription());
            //    ApiDataService dataService = new ApiDataService(sql);
            //};
        }



        private dynamic RouteSearch(string searchQuery, int page, string rarity, string category, string faction, string rItems, string mItems, int id)
        {
            if (searchQuery == null)
            {
                searchQuery = "";
            }

            //ApiDataService db = new ApiDataService(sql);

            sql.Open(WebSettings.Settings.CreateDescription());

            var parmeter = new List<Parameter>();

            bool hasFilter = !string.IsNullOrEmpty(searchQuery);

            FilterModel filterModel = new FilterModel
            {
                Categories = ApiDataService.SelectCategories(sql),
                Rarities = ApiDataService.SelectRarities(sql),
                Factions = ApiDataService.SelectFactions(sql),
            };

            var rarityItem = filterModel.VerifyRarity(rarity);
            var categoryItem = filterModel.VerifyCategory(category);
            var factionItem = filterModel.VerifyFaction(faction);
            var showRemovedItems = filterModel.VerifyRmdItems(rItems);
            var showMetaItems = filterModel.VerifyMetaItems(mItems);

            filterModel.CurrentShowRemovedItems = showRemovedItems;
            filterModel.CurrentShowMetaItems = showMetaItems;

            string sqlQuery = DataService.BuildSearchQuery(hasFilter, true, false, id > 0, rarityItem != null, categoryItem != null, factionItem != null, showRemovedItems, showMetaItems);

            if (hasFilter)
            {
                var p = new Parameter { Identifier = "@filter", Value = $"%{searchQuery}%" };
                parmeter.Add(p);
            }

            if (rarityItem != null)
            {
                var p = new Parameter { Identifier = "@rarity", Value = $"{rarityItem.Id}" };
                parmeter.Add(p);
            }

            if (categoryItem != null)
            {
                var p = new Parameter { Identifier = "@category", Value = $"{categoryItem.Id}" };
                parmeter.Add(p);
            }

            if (factionItem != null)
            {
                var p = new Parameter { Identifier = "@faction", Value = $"{factionItem.Id}" };
                parmeter.Add(p);
            }

            if (id > 0)
            {
                var p = new Parameter { Identifier = "@id", Value = $"{id}" };
                parmeter.Add(p);
            }
            
            var ds = sql.SelectDataSet(sqlQuery, parmeter);
            var searchResult = new List<Item>();
            foreach (var row in ds)
            {
                Item item = Item.Create(row);
                CrossoutDataService.Instance.AddData(item);
                searchResult.Add(item);
            }

            //var str = JsonConvert.SerializeObject(searchResult);

            return Response.AsJson(searchResult);
        }

        
    }
}
