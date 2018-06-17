using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Models;
using Crossout.Web.Models.Filter;
using Crossout.Web.Models.General;
using Crossout.Web.Models.Pagination;
using Crossout.Web.Services;
using Nancy;
using Nancy.Responses;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.Model;

namespace Crossout.Web.Modules.Search
{
    public class CarftingOverviewModule : NancyModule
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        public CarftingOverviewModule(IRootPathProvider rootPathProvider)
        {
            Get["/crafting"] = x =>
            {
                return RouteCraftingOverview();
            };
        }

        private dynamic RouteCraftingOverview()
        {
            DataService db = new DataService(sql);

            sql.Open(WebSettings.Settings.CreateDescription());

            FilterModel filterModel = new FilterModel
            {
                Categories = SelectCategories(sql),
                Rarities = SelectRarities(sql),
                Factions = SelectFactions(sql),
            };

            string sqlQuery = DataService.BuildCraftingOverviewQuery();

            var ds = sql.SelectDataSet(sqlQuery);
            var items = new List<Item>();
            foreach (var row in ds)
            {
                Item item = Item.Create(row);
                items.Add(item);
            }

            CraftingOverviewModel carftingOverviewModel = new CraftingOverviewModel { Items = items, FilterModel = filterModel };

            var statusModel = db.SelectStatus();
            carftingOverviewModel.Status = statusModel;

            return View["crafting", carftingOverviewModel];
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
