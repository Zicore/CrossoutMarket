using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.Model.Items;
using Crossout.Web;
using Crossout.AspWeb.Models.Filter;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Models.Pagination;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.AspWeb.Models.Language;

namespace Crossout.AspWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public HomeController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("")]
        public IActionResult Index(bool rmdItems)
        {
            this.RegisterHit("Search");
            return RouteSearchAjax();
            //return RouteSearch(null, 0, null, null, null, null, null, rmdItems);
            //return RouteSearch(null, 0,null,null,null,null,null);
        }


        [Route("search")]
        public IActionResult Search(string query, string rarity, string category, string faction, string rmditems, string mitems)
        {
            return Redirect("/");
            //return RouteSearch(query, 0, rarity, category, faction, rmditems, mitems);
        }

        private IActionResult RouteSearchAjax()
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            Language lang = this.ReadLanguageCookie(sql);

            var parmeter = new List<Parameter>();

            FilterModel filterModel = new FilterModel
            {
                Categories = SelectCategories(sql),
                Rarities = SelectRarities(sql),
                Factions = SelectFactions(sql),
            };

            SearchModel searchModel = new SearchModel { FilterModel = filterModel };

            var statusModel = db.SelectStatus();
            searchModel.Status = statusModel;

            searchModel.Localizations = db.SelectFrontendLocalizations(lang.Id, "search");

            return View("search", searchModel);
        }

        private IActionResult RouteSearch(string searchQuery, int page, string rarity, string category, string faction, string rItems, string mItems, bool rmdItemsOnly)
        {
            if (searchQuery == null)
            {
                searchQuery = "";
            }

            DataService db = new DataService(sql);

            sql.Open(WebSettings.Settings.CreateDescription());

            var parmeter = new List<Parameter>();

            bool hasFilter = !string.IsNullOrEmpty(searchQuery);

            FilterModel filterModel = new FilterModel
            {
                Categories = SelectCategories(sql),
                Rarities = SelectRarities(sql),
                Factions = SelectFactions(sql),
            };

            var rarityItem = filterModel.VerifyRarity(rarity);
            var categoryItem = filterModel.VerifyCategory(category);
            var factionItem = filterModel.VerifyFaction(faction);
            var showRemovedItems = filterModel.VerifyRmdItems(rItems);
            var showMetaItems = filterModel.VerifyMetaItems(mItems);

            filterModel.CurrentShowRemovedItems = showRemovedItems;
            filterModel.CurrentShowMetaItems = showMetaItems;
            filterModel.CurrentShowRemovedItemsOnly = rmdItemsOnly;

            string sqlQuery = DataService.BuildSearchQuery(hasFilter, true, false, false, rarityItem != null, categoryItem != null, factionItem != null, false, true, rmdItemsOnly);

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

            page = Math.Max(1, page);
            int entriesPerPage = 50;
            int from = entriesPerPage * (page - 1);

            var limita = new Parameter { Identifier = "@from", Value = from };
            var limitb = new Parameter { Identifier = "@to", Value = entriesPerPage };

            parmeter.Add(limita);
            parmeter.Add(limitb);


            var count = GetCount(sql, hasFilter, parmeter, rarityItem, categoryItem, factionItem, showRemovedItems, showMetaItems);

            int maxPages = (int)Math.Ceiling(count / (float)entriesPerPage);

            var ds = sql.SelectDataSet(sqlQuery, parmeter);
            var searchResult = new List<Item>();
            foreach (var row in ds)
            {
                Item item = Item.Create(row);
                item.SetImageExists(pathProvider);
                // CrossoutDataService.Instance.AddData(item);
                searchResult.Add(item);
            }
            //  CurrentPage = page, MaxRows = count, MaxPages = maxPages

            PagerModel pager = new PagerModel
            {
                CurrentPage = page,
                MaxRows = count,
                MaxPages = maxPages
            };
            SearchModel searchModel = new SearchModel { SearchResult = searchResult, Pager = pager, FilterModel = filterModel, CurrentQuery = searchQuery };

            var statusModel = db.SelectStatus();
            searchModel.Status = statusModel;

            return View("search", searchModel);
        }

        // Helper Methods: TODO: Move to seperate class

        public static int GetCount(SqlConnector sql, bool hasFilter, List<Parameter> parameter, RarityItem rarityItem, FilterItem categoryItem, FilterItem factionItem, bool showRemovedItems, bool showMetaItems)
        {
            string countQuery = DataService.BuildSearchQuery(hasFilter, false, true, false, rarityItem != null, categoryItem != null, factionItem != null, showRemovedItems, showMetaItems, false);
            var countDS = sql.SelectDataSet(countQuery, parameter);
            int count = 0;
            if (countDS != null && countDS.Count > 0)
            {
                count = Convert.ToInt32(countDS[0][0]);
            }
            return count;
        }


        public static List<RarityItem> SelectRarities(SqlConnector sql)
        {
            List<RarityItem> items = new List<RarityItem>();

            var ds = sql.SelectDataSet("SELECT rarity.id, rarity.name, rarity.order, rarity.primarycolor, rarity.secondarycolor FROM rarity ORDER BY rarity.order ASC");

            foreach (var row in ds)
            {
                items.Add(RarityItem.Create(row));
            }

            return items;
        }

        public static List<FilterItem> SelectFactions(SqlConnector sql)
        {
            List<FilterItem> items = new List<FilterItem>();

            var ds = sql.SelectDataSet("SELECT id,name FROM faction");

            foreach (var row in ds)
            {
                items.Add(FilterItem.Create(row));
            }

            return items;
        }

        public static List<FilterItem> SelectCategories(SqlConnector sql)
        {
            List<FilterItem> items = new List<FilterItem>();

            var ds = sql.SelectDataSet("SELECT id,name FROM category");

            foreach (var row in ds)
            {
                items.Add(FilterItem.Create(row));
            }

            return items;
        }
    }
}