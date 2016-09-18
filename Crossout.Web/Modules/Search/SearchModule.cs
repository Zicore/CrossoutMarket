using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Models;
using Crossout.Web.Models.Filter;
using Crossout.Web.Models.Pagination;
using Nancy;
using Nancy.Responses;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Modules.Search
{
    public class SearchModule : NancyModule
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        public SearchModule()
        {
            Get["/"] = x =>
            {
                return RouteSearch(null, 0,null,null);
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
                var query = (string)Request.Query.Query;

                int page = x.page;
                return RouteSearch(query, page, rarity, category);
            };

            Get["/{page:int}"] = x =>
            {
                int page = x.page;
                return RouteSearch(null, page, null,null);
            };
        }

        private dynamic RouteSearch(string searchQuery, int page, string rarity, string category)
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
                Categories = SelectCategories(sql),
                Rarities = SelectRarities(sql)
            };

            var rarityItem = filterModel.VerifyRarity(rarity);
            var categoryItem = filterModel.VerifyCategory(category);

            
            
            string sqlQuery = BuildSearchQuery(hasFilter, true,false,false, rarityItem != null, categoryItem != null);

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

            page = Math.Max(1, page);
            int entriesPerPage = 50;
            int from = entriesPerPage * (page - 1);

            var limita = new Parameter { Identifier = "@from", Value = from };
            var limitb = new Parameter { Identifier = "@to", Value = entriesPerPage };

            parmeter.Add(limita);
            parmeter.Add(limitb);


            var count = GetCount(sql, hasFilter, parmeter, rarityItem, categoryItem);

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
            
            return View["search", searchModel];
        }

        // Helper Methods: TODO: Move to seperate class

        public static int GetCount(SqlConnector sql,bool hasFilter, List<Parameter> parameter, FilterItem rarityItem, FilterItem categoryItem)
        {
            string countQuery = BuildSearchQuery(hasFilter, false, true, false, rarityItem != null, categoryItem != null);
            var countDS = sql.SelectDataSet(countQuery, parameter);
            int count = 0;
            if (countDS != null && countDS.Count > 0)
            {
                count = Convert.ToInt32(countDS[0][0]);
            }
            return count;
        }

        public static string BuildSearchQuery(bool hasFilter, bool limit, bool count, bool hasId, bool hasRarity, bool hasCategory)
        {
            string selectColumns = "item.id,item.name,m1.sellprice,m1.buyprice,m1.selloffers,m1.buyorders,m1.datetime,rarity.id,rarity.name,category.id,category.name,type.id,type.name,recipe.id ";
            if (count)
            {
                selectColumns = "count(*)";
            }
            string query = $"SELECT {selectColumns} FROM market m1 JOIN(SELECT itemnumber, MAX(datetime) datetime FROM market GROUP BY itemnumber) m2 ON m1.itemnumber = m2.itemnumber AND m1.datetime = m2.datetime LEFT JOIN item on item.id = m1.itemnumber  LEFT JOIN rarity on rarity.id = item.raritynumber LEFT JOIN category on category.id = item.categorynumber LEFT JOIN type on type.id = item.typenumber LEFT JOIN recipe ON recipe.itemnumber = item.id ";

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

            if (!count)
            {
                query += "ORDER BY item.id asc, item.name asc ";
            }

            if (limit)
            {
                query += "LIMIT @from,@to";
            }

            return query;
        }

        public static List<FilterItem> SelectRarities(SqlConnector sql)
        {
            List<FilterItem> items = new List<FilterItem>();

            var ds = sql.SelectDataSet("SELECT id,name FROM Rarity");

            foreach (var row in ds)
            {
                items.Add(FilterItem.Create(row));
            }

            return items;
        }

        public static List<FilterItem> SelectCategories(SqlConnector sql)
        {
            List<FilterItem> items = new List<FilterItem>();

            var ds = sql.SelectDataSet("SELECT id,name FROM Category");

            foreach (var row in ds)
            {
                items.Add(FilterItem.Create(row));
            }

            return items;
        }
    }
}
