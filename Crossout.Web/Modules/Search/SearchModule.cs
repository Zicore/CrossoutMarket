using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Models;
using Crossout.Web.Models.Filter;
using Crossout.Web.Models.Pagination;
using Crossout.Web.Services;
using Nancy;
using Nancy.Responses;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Modules.Search
{
    public class SearchModule : NancyModule
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        public SearchModule(IRootPathProvider rootPathProvider)
        {
            Get["/"] = x =>
            {
                return RouteSearch(null, 0,null,null,null);
            };

            //Get["/search/{page?}"] = x =>
            //{
            //    string rarity = (string)Request.Query.Rarity;
            //    string category = (string)Request.Query.Category;
            //    var filter = (string)Request.Query.Query;
            //    int page = x.page;
            //    return RouteSearch(filter, page, rarity, category);
            //};

            Get["/search/{page?}"] = x =>
            {
                string rarity = (string)Request.Query.Rarity;
                string category = (string)Request.Query.Category;
                string faction = (string)Request.Query.Faction;
                var query = (string)Request.Query.Query;

                int page = x.page;
                return RouteSearch(query, page, rarity, category, faction);
            };

            Get["/{page:int}"] = x =>
            {
                int page = x.page;
                return RouteSearch(null, page, null,null,null);
            };
        }

        private dynamic RouteSearch(string searchQuery, int page, string rarity, string category, string faction)
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
                Factions = SelectFactions(sql)
            };

            var rarityItem = filterModel.VerifyRarity(rarity);
            var categoryItem = filterModel.VerifyCategory(category);
            var factionItem = filterModel.VerifyFaction(faction);


            string sqlQuery = DataService.BuildSearchQuery(hasFilter, true,false,false, rarityItem != null, categoryItem != null, factionItem != null);

            if (hasFilter)
            {
                var p = new Parameter {Identifier = "@filter", Value = $"%{searchQuery}%"};
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


            var count = GetCount(sql, hasFilter, parmeter, rarityItem, categoryItem, factionItem);

            int maxPages = (int)Math.Ceiling(count / (float)entriesPerPage);

            var ds = sql.SelectDataSet(sqlQuery, parmeter);
            var searchResult = new List<Item>();
            foreach (var row in ds)
            {
                Item item = Item.Create(row);
                searchResult.Add(item);
            }
            //  CurrentPage = page, MaxRows = count, MaxPages = maxPages
            
            PagerModel pager = new PagerModel
            {
                CurrentPage = page,
                MaxRows = count,
                MaxPages = maxPages
            };
            SearchModel searchModel = new SearchModel {SearchResult = searchResult, Pager = pager, FilterModel = filterModel, CurrentQuery = searchQuery};

            var statusModel = db.SelectStatus();
            searchModel.Status = statusModel;

            return View["search", searchModel];
        }

        // Helper Methods: TODO: Move to seperate class

        public static int GetCount(SqlConnector sql,bool hasFilter, List<Parameter> parameter, FilterItem rarityItem, FilterItem categoryItem, FilterItem factionItem)
        {
            string countQuery = DataService.BuildSearchQuery(hasFilter, false, true, false, rarityItem != null, categoryItem != null, factionItem != null);
            var countDS = sql.SelectDataSet(countQuery, parameter);
            int count = 0;
            if (countDS != null && countDS.Count > 0)
            {
                count = Convert.ToInt32(countDS[0][0]);
            }
            return count;
        }


        public static List<FilterItem> SelectRarities(SqlConnector sql)
        {
            List<FilterItem> items = new List<FilterItem>();

            var ds = sql.SelectDataSet("SELECT id,name FROM rarity");

            foreach (var row in ds)
            {
                items.Add(FilterItem.Create(row));
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
