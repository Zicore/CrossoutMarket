﻿using System;
using System.Collections.Generic;
using System.Linq;
using Crossout.Model.Items;
using Crossout.Web.Models.API.v1;
using Crossout.Web.Models.Charts;
using Crossout.Web.Models.Filter;
using Crossout.Web.Models.General;
using Crossout.Web.Models.Pagination;
using Crossout.Web.Services;
using Crossout.Web.Services.API.v1;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Modules.API.v1
{
    public class ApiModule : NancyModule
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        public ApiModule()
        {
            OnError += (ctx, exp) =>
            {
                var modelBindingException = exp as ModelBindingException;
                if (modelBindingException != null)
                {
                    var errorModel = new
                    {
                        ErrorMessage = modelBindingException.Message,
                        PropertyBindingErrors = modelBindingException.PropertyBindingExceptions?.Select(x => new
                        {
                            ErrorMessage = x.Message,
                            InnerException = x.InnerException?.Message
                        }).ToArray()
                    };
                    return Response.AsJson(errorModel, HttpStatusCode.BadRequest);
                }

                return null;
            };

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
                string showRemovedItems = (string)Request.Query.RemovedItems;
                string showMetaItems = (string)Request.Query.MetaItems;
                var query = (string)Request.Query.Query;
                int id = (int) Request.Query.Id;
                
                return RouteSearch(query, 0, rarity, category, faction, showRemovedItems, showMetaItems, id);
            };

            Get["/api/v1/item/{item:int}"] = x =>
            {
                string rarity = (string)Request.Query.Rarity;
                string category = (string)Request.Query.Category;
                string faction = (string)Request.Query.Faction;
                string showRemovedItems = (string)Request.Query.RemovedItems;
                string showMetaItems = (string)Request.Query.MetaItems;
                var query = (string)Request.Query.Query;
                int id = (int)x.item;

                return RouteSearch(query, 0, rarity, category, faction, showRemovedItems, showMetaItems, id);
            };
            
            Get["/api/v1/recipe/{item:int}"] = x =>
            {
                var id = (int)x.item;

                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemModel = db.SelectItem(id, true);
                var recipeModel = db.SelectRecipeModel(itemModel.Item, false);
                
                return Response.AsJson(recipeModel);
            };

            Get["/api/v1/recipe-deep/{item:int}"] = x =>
            {
                var id = (int)x.item;
                
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemModel = db.SelectItem(id, true);
                var recipeModel = db.SelectRecipeModel(itemModel.Item, true);

                itemModel.Recipe = recipeModel;

                return Response.AsJson(itemModel);
            };

            Get["/api/v1/market/{name}/{id:int}"] = x =>
            {
                sql.Open(WebSettings.Settings.CreateDescription());

                string name = x.name;
                HashSet<string> validMarkets = new HashSet<string>() { "sellprice", "buyprice", "selloffers", "buyorders" };

                bool unixTimeStamp = (bool)Request.Query.unixTimestamp;

                string timestampColumn = "market.datetime";

                if (validMarkets.Contains(name))
                {
                    if (unixTimeStamp)
                    {
                        timestampColumn = "UNIX_TIMESTAMP(CONVERT_TZ(market.datetime, '+00:00', @@global.time_zone))";
                    }

                    string query = $"(SELECT {timestampColumn},market.{name} FROM market where market.itemnumber = @id ORDER BY market.Datetime desc LIMIT 40000);";

                    var parmeter = new List<Parameter>
                    {
                        new Parameter {Identifier = "@id", Value = x.id},
                    };
                    var ds = sql.SelectDataSet(query, parmeter);
                    return Response.AsJson(ds);
                }
                else
                {
                    return Response.AsJson("Market not found", HttpStatusCode.NotFound);
                }
            };

            Get["/api/v1/market-all/{id:int}"] = x =>
            {
                sql.Open(WebSettings.Settings.CreateDescription());

                var request = this.Bind<MarketAllRequest>();

                var startTimestamp = request.StartTimestamp;
                var endTimestamp = request.EndTimestamp;

                if (startTimestamp.HasValue && startTimestamp < 0)
                    return Response.AsJson(new { ErrorMessage = "Parameter startTimestamp should be positive integer less than or equal to " + int.MaxValue }, HttpStatusCode.BadRequest);
                if (endTimestamp.HasValue && endTimestamp < 0)
                    return Response.AsJson(new { ErrorMessage = "Parameter endTimestamp should be positive integer less than or equal to " + int.MaxValue }, HttpStatusCode.BadRequest);

                var whereClause = "where market.itemnumber = @id";

                if (startTimestamp.HasValue && endTimestamp.HasValue)
                {
                    whereClause += " AND market.datetime BETWEEN CONVERT_TZ(FROM_UNIXTIME(@startTimestamp), @@global.time_zone, '+00:00') AND CONVERT_TZ(FROM_UNIXTIME(@endTimestamp), @@global.time_zone, '+00:00')";
                }
                else
                {
                    if (startTimestamp.HasValue)
                        whereClause += " AND market.datetime >= CONVERT_TZ(FROM_UNIXTIME(@startTimestamp), @@global.time_zone, '+00:00')";
                    if (endTimestamp.HasValue)
                        whereClause += " AND market.datetime <= CONVERT_TZ(FROM_UNIXTIME(@endTimestamp), @@global.time_zone, '+00:00')";
                }

                string query = "(" +
                               "SELECT market.id,market.sellprice,market.buyprice,market.selloffers,market.buyorders,market.datetime,UNIX_TIMESTAMP(CONVERT_TZ(market.datetime, '+00:00', @@global.time_zone)) as unixdatetime " +
                               "FROM market " +
                               $"{whereClause} " +
                               "ORDER BY market.Datetime desc LIMIT 40000" +
                               ") ORDER BY id ASC;";

                var p = new Parameter { Identifier = "@id", Value = request.Id };
                var parmeter = new List<Parameter>();
                parmeter.Add(p);

                if (startTimestamp.HasValue)
                    parmeter.Add(new Parameter("startTimestamp", startTimestamp.Value));
                if (endTimestamp.HasValue)
                    parmeter.Add(new Parameter("endTimestamp", endTimestamp.Value));

                var ds = sql.SelectDataSet(query, parmeter);
                
                return Response.AsJson(ds);
            };

            After.AddItemToEndOfPipeline((ctx) =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                    .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");
            });
        }
        
        private dynamic RouteSearch(string searchQuery, int page, string rarity, string category, string faction, string rItems, string mItems, int id)
        {
            if (searchQuery == null)
            {
                searchQuery = "";
            }

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
