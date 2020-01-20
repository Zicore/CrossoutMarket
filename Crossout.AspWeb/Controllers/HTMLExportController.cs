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
    public class HtmlExportController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public HtmlExportController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        [Route("export")]
        public IActionResult Export(bool showTable, bool image, bool name, bool rarity, bool faction, bool category, bool type, bool popularity, bool sellPrice, bool sellOffers, bool buyPrice, bool buyOrders, bool margin, bool lastUpdate, bool craftingCostSell, bool craftingCostBuy, bool craftingMargin, bool craftVsBuy, bool link)
        {
            return RouteHtmlExport(showTable, image, name, rarity, faction, category, type, popularity, sellPrice, sellOffers, buyPrice, buyOrders, margin, lastUpdate, craftingCostSell, craftingCostBuy, craftingMargin, craftVsBuy, link);
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private IActionResult RouteHtmlExport(bool showTable, bool showImage, bool showName, bool showRarity, bool showFaction, bool showCategory, bool showType, bool showPopulartiy, bool showSellPrice, bool showSellOffers, bool showBuyPrice, bool showBuyOrders, bool showMargin, bool showLastUpdate, bool showCraftingCostSell, bool showCraftingCostBuy, bool showCraftingMargin, bool showCraftVsBuy, bool showLink)
        {
            DataService db = new DataService(sql);

            sql.Open(WebSettings.Settings.CreateDescription());

            FilterModel filterModel = new FilterModel
            {
                Categories = SelectCategories(sql),
                Rarities = SelectRarities(sql),
                Factions = SelectFactions(sql),
            };

            string sqlQuery = DataService.BuildHtmlExport();

            var ds = sql.SelectDataSet(sqlQuery);
            var items = new List<Item>();
            foreach (var row in ds)
            {
                Item item = Item.Create(row);
                item.SetImageExists(pathProvider);
                items.Add(item);
            }

            HtmlExportModel htmlExportModel = new HtmlExportModel { 
                Items = items,
                FilterModel = filterModel,

                ShowTable = showTable,
                ShowImage = showImage,
                ShowName = showName,
                ShowRarity = showRarity,
                ShowFaction = showFaction,
                ShowCategory = showCategory,
                ShowType = showType,
                ShowPopulartiy = showPopulartiy,
                ShowSellPrice = showSellPrice,
                ShowSellOffers = showSellOffers,
                ShowBuyPrice = showBuyPrice,
                ShowBuyOrders = showBuyOrders,
                ShowMargin = showMargin,
                ShowLastUpdate = showLastUpdate,
                ShowCraftingCostSell = showCraftingCostSell,
                ShowCraftingCostBuy = showCraftingCostBuy,
                ShowCraftingMargin = showCraftingMargin,
                ShowCraftVsBuy = showCraftVsBuy,
                ShowLink = showLink
            };

            var statusModel = db.SelectStatus();
            htmlExportModel.Status = statusModel;

            return View("export", htmlExportModel);
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