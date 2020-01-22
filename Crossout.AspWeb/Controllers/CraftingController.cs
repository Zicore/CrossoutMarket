using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.Model.Items;
using Crossout.Web;
using Crossout.AspWeb.Models.Filter;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.AspWeb.Controllers
{
    public class CraftingController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public CraftingController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        [Route("crafting")]
        public IActionResult Crafting()
        {
            // Redirect for legacy links
            return Redirect("/#preset=crafting");
            //return RouteCraftingOverview();
        }

        //SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        //private IActionResult RouteCraftingOverview()
        //{
        //    DataService db = new DataService(sql);

        //    sql.Open(WebSettings.Settings.CreateDescription());

        //    FilterModel filterModel = new FilterModel
        //    {
        //        Categories = SelectCategories(sql),
        //        Rarities = SelectRarities(sql),
        //        Factions = SelectFactions(sql),
        //    };

        //    string sqlQuery = DataService.BuildCraftingOverviewQuery();

        //    var ds = sql.SelectDataSet(sqlQuery);
        //    var items = new List<Item>();
        //    foreach (var row in ds)
        //    {
        //        Item item = Item.Create(row);
        //        item.SetImageExists(pathProvider);
        //        items.Add(item);
        //    }

        //    CraftingOverviewModel carftingOverviewModel = new CraftingOverviewModel { Items = items, FilterModel = filterModel };

        //    var statusModel = db.SelectStatus();
        //    carftingOverviewModel.Status = statusModel;

        //    return View("crafting", carftingOverviewModel);
        //}

        //public static List<FilterItem> SelectRarities(SqlConnector sql)
        //{
        //    List<FilterItem> items = new List<FilterItem>();

        //    var ds = sql.SelectDataSet("SELECT id,name FROM rarity");

        //    foreach (var row in ds)
        //    {
        //        items.Add(FilterItem.Create(row));
        //    }

        //    return items;
        //}

        //public static List<FilterItem> SelectFactions(SqlConnector sql)
        //{
        //    List<FilterItem> items = new List<FilterItem>();

        //    var ds = sql.SelectDataSet("SELECT id,name FROM faction");

        //    foreach (var row in ds)
        //    {
        //        items.Add(FilterItem.Create(row));
        //    }

        //    return items;
        //}

        //public static List<FilterItem> SelectCategories(SqlConnector sql)
        //{
        //    List<FilterItem> items = new List<FilterItem>();

        //    var ds = sql.SelectDataSet("SELECT id,name FROM category");

        //    foreach (var row in ds)
        //    {
        //        items.Add(FilterItem.Create(row));
        //    }

        //    return items;
        //}
    }
}