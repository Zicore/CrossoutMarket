using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model;
using Crossout.Web.Models.API.v1;
using Crossout.Web.Models.Filter;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Services.API.v1
{
    public class ApiDataService
    {
        protected SqlConnector DB { get; set; }

        public ApiDataService(SqlConnector sql)
        {
            DB = sql;
        }

        public List<T> CreateApiEntryBase<T>(List<object[]> ds) where T : ApiEntryBase, new()
        {
            var result = new List<T>();

            foreach (var row in ds)
            {
                var entry = new T
                {
                    Id = row[0].ConvertTo<int>(),
                    Name = row[1].ConvertTo<string>(),
                };

                result.Add(entry);
            }

            return result;
        }

        public List<ApiRarityEntry> GetRarities()
        {
            string query = "SELECT id, name FROM rarity ORDER BY id ASC;";
            var ds = DB.SelectDataSet(query);
            var list = CreateApiEntryBase<ApiRarityEntry>(ds);
            return list;
        }

        public List<ApiFactionEntry> GetFactions()
        {
            string query = "SELECT id, name FROM faction ORDER BY id ASC;";
            var ds = DB.SelectDataSet(query);
            var list = CreateApiEntryBase<ApiFactionEntry>(ds);
            return list;
        }

        public List<ApiCategoryEntry> GetCategories()
        {
            string query = "SELECT id, name FROM category ORDER BY id ASC;";
            var ds = DB.SelectDataSet(query);
            var list = CreateApiEntryBase<ApiCategoryEntry>(ds);
            return list;
        }

        public List<ApiCategoryEntry> GetItemTypes()
        {
            string query = "SELECT id, name FROM type ORDER BY id ASC;";
            var ds = DB.SelectDataSet(query);
            var list = CreateApiEntryBase<ApiCategoryEntry>(ds);
            return list;
        }

        public static ApiItemEntry CreateApiItem(object[] row)
        {
            int i = 0;
            ApiItemEntry item = new ApiItemEntry
            {
                Id = row[i++].ConvertTo<int>(),
                Name = row[i++].ConvertTo<string>(),
                SellPrice = row[i++].ConvertTo<decimal>(),
                BuyPrice = row[i++].ConvertTo<decimal>(),
                SellOffers = row[i++].ConvertTo<int>(),
                BuyOrders = row[i++].ConvertTo<int>(),
                Timestamp = row[i++].ConvertTo<DateTime>(),
                RarityId = row[i++].ConvertTo<int>(),
                RarityName = row[i++].ConvertTo<string>(),
                CategoryId = row[i++].ConvertTo<int>(),
                CategoryName = row[i++].ConvertTo<string>(),
                TypeId = row[i++].ConvertTo<int>(),
                TypeName = row[i++].ConvertTo<string>(),
                RecipeId = row[i++].ConvertTo<int>(),
                Removed = row[i++].ConvertTo<int>(),
                FactionNumber = row[i++].ConvertTo<int>(),
                Faction = row[i++].ConvertTo<string>(),
                Popularity = row[i++].ConvertTo<int>(),
                WorkbenchRarity = row[i].ConvertTo<int>(),
            };

            return item;
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

        public static string BuildSearchQuery(bool hasFilter, bool limit, bool count, bool hasId, bool hasRarity, bool hasCategory, bool hasFaction, bool showRemovedItems, bool showMetaItems)
        {
            string selectColumns = "item.id,item.name,item.sellprice,item.buyprice,item.selloffers,item.buyorders,item.datetime,rarity.id,rarity.name,category.id,category.name,type.id,type.name,recipe.id,item.removed,faction.id,faction.name,item.popularity,item.workbenchrarity";
            if (count)
            {
                selectColumns = "count(*)";
            }
            string query = $"SELECT {selectColumns} FROM item LEFT JOIN rarity on rarity.id = item.raritynumber LEFT JOIN category on category.id = item.categorynumber LEFT JOIN type on type.id = item.typenumber LEFT JOIN recipe ON recipe.itemnumber = item.id LEFT JOIN faction ON faction.id = recipe.factionnumber ";

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

            if (hasFaction)
            {
                query += " AND faction.id = @faction ";
            }

            if (!showRemovedItems)
            {
                query += " AND item.removed = 0 ";
            }

            if (!showMetaItems)
            {
                query += " AND item.meta = 0 ";
            }

            if (!count)
            {
                query += "ORDER BY item.id asc, item.name asc ";
            }
            
            return query;
        }
    }
}
