using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Crossout.AspWeb.Filters;
using Crossout.Model.Items;
using Crossout.AspWeb;
using Crossout.AspWeb.Models.Filter;
using Crossout.AspWeb.Services;
using Crossout.AspWeb.Services.API.v1;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.Web.Modules.API.v1;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.Language;

namespace Crossout.AspWeb.Controllers
{
    [AddHeader("Access-Control-Allow-Origin", "*")]
    [AddHeader("Access-Control-Allow-Methods", "POST,GET")]
    [AddHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type")]
    public class ApiController : Controller
    {
        private readonly RootPathHelper pathProvider;
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        public ApiController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        [Route("/api/v1/languages")]
        public IActionResult LanguagesAction()
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            LanguageModel model = db.SelectLanguageModel();

            this.RegisterHit("/api/v1/languages");
            return Json(model);
        }

        [Route("/api/v1/rarities")]
        public IActionResult RaritiesAction()
        {
            sql.Open(WebSettings.Settings.CreateDescription());
            ApiDataService dataService = new ApiDataService(sql);

            var model = dataService.GetRarities();

            this.RegisterHit("/api/v1/rarities");
            return Json(model);
        }
        
        [Route("/api/v1/factions")]
        public IActionResult FactionsAction()
        {
            sql.Open(WebSettings.Settings.CreateDescription());
            ApiDataService dataService = new ApiDataService(sql);

            var model = dataService.GetFactions();

            this.RegisterHit("/api/v1/factions");
            return Json(model);
        }

        [Route("/api/v1/types")]
        public IActionResult TypesAction()
        {
            sql.Open(WebSettings.Settings.CreateDescription());
            ApiDataService dataService = new ApiDataService(sql);

            var model = dataService.GetItemTypes();

            this.RegisterHit("/api/v1/types");
            return Json(model);
        }

        [Route("/api/v1/categories")]
        public IActionResult CategoriesAction()
        {
            sql.Open(WebSettings.Settings.CreateDescription());
            ApiDataService dataService = new ApiDataService(sql);

            var model = dataService.GetCategories();

            this.RegisterHit("/api/v1/categories");
            return Json(model);
        }

        //[Route("/api/v1/items")]
        //public IActionResult ItemsAllAction()
        //{
        //    return RouteSearch(null, 0, null, null, null, null, null, 0);
        //}

        [Route("/api/v1/items")]
        public IActionResult ItemsAllSearchAction(string query, string rarity, string category, string faction, string removedItems, string metaItems, int id, string language)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            LanguageModel languageModel = db.SelectLanguageModel();
            Language selectedLanguage = languageModel.VerifyLanguage(language);

            this.RegisterHit("/api/v1/items");
            return RouteSearch(query, 0, rarity, category, faction, removedItems, metaItems, id, selectedLanguage.Id);
        }

        [Route("/api/v1/item/{item}")]
        public IActionResult ItemAction(string query, string rarity, string category, string faction, string removedItems, string metaItems, int item, string language)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            LanguageModel languageModel = db.SelectLanguageModel();
            Language selectedLanguage = languageModel.VerifyLanguage(language);

            this.RegisterHit("/api/v1/item");
            return RouteSearch(query, 0, rarity, category, faction, removedItems, metaItems, item, selectedLanguage.Id);
        }

        [Route("/api/v1/recipe/{item}")]
        public IActionResult RecipeAction(int item, string language)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            LanguageModel languageModel = db.SelectLanguageModel();
            Language selectedLanguage = languageModel.VerifyLanguage(language);

            var itemModel = db.SelectItem(item, true, selectedLanguage.Id);
            var recipeModel = db.SelectRecipeModel(itemModel.Item, false, selectedLanguage.Id);

            this.RegisterHit("/api/v1/recipe");
            return Json(recipeModel);
        }

        [Route("/api/v1/recipe-deep/{item}")]
        public IActionResult RecipeDeepAction(int item, string language)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            LanguageModel languageModel = db.SelectLanguageModel();
            Language selectedLanguage = languageModel.VerifyLanguage(language);

            var itemModel = db.SelectItem(item, true, selectedLanguage.Id);
            var recipeModel = db.SelectRecipeModel(itemModel.Item, true, selectedLanguage.Id);

            itemModel.Recipe = recipeModel;

            this.RegisterHit("/api/v1/recipe-deep");
            return Json(itemModel);
        }

        [Route("/api/v1/market/{name}/{id}")]
        public IActionResult MarketAction(string name, int id, bool unixTimeStamp)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            HashSet<string> validMarkets = new HashSet<string>() { "sellprice", "buyprice", "selloffers", "buyorders" };
            
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
                    new Parameter {Identifier = "@id", Value = id},
                };
                var ds = sql.SelectDataSet(query, parmeter);

                this.RegisterHit("/api/v1/market");
                return Json(ds);
            }
            else
            {
                return Json("Market not found", HttpStatusCode.NotFound);
            }
        }

        [Route("/api/v1/market-all/{id:int}")]
        public IActionResult MarketAllAction(MarketAllRequest request)
        {
            sql.Open(WebSettings.Settings.CreateDescription());
            
            var startTimestamp = request.StartTimestamp;
            var endTimestamp = request.EndTimestamp;

            if (startTimestamp.HasValue && startTimestamp < 0)
                return Json(new { ErrorMessage = "Parameter startTimestamp should be positive integer less than or equal to " + int.MaxValue }, HttpStatusCode.BadRequest);
            if (endTimestamp.HasValue && endTimestamp < 0)
                return Json(new { ErrorMessage = "Parameter endTimestamp should be positive integer less than or equal to " + int.MaxValue }, HttpStatusCode.BadRequest);

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

            this.RegisterHit("/api/v1/market-all");
            return Json(ds);
        }

        private IActionResult RouteSearch(string searchQuery, int page, string rarity, string category, string faction, string removedItems, string metaItems, int id, int language)
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
            var showRemovedItems = filterModel.VerifyRmdItems(removedItems);
            var showMetaItems = filterModel.VerifyMetaItems(metaItems);

            filterModel.CurrentShowRemovedItems = showRemovedItems;
            filterModel.CurrentShowMetaItems = showMetaItems;

            string sqlQuery = DataService.BuildSearchQuery(hasFilter, true, false, id > 0, rarityItem != null, categoryItem != null, factionItem != null, showRemovedItems, showMetaItems, false);

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

            parmeter.Add(new Parameter { Identifier = "@language", Value = language });

            var ds = sql.SelectDataSet(sqlQuery, parmeter);
            var searchResult = new List<Item>();
            foreach (var row in ds)
            {
                Item item = Item.Create(row);
                CrossoutDataService.Instance.AddData(item);
                searchResult.Add(item);
            }

            //var str = JsonConvert.SerializeObject(searchResult);

            return Json(searchResult);
        }
    }
}