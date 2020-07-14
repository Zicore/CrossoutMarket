using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.Model.Formatter;
using Crossout.AspWeb;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.AspWeb.Models.Language;

namespace Crossout.AspWeb.Controllers
{
    public class EventController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public EventController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        [Route("event")]
        public IActionResult Event()
        {
            this.RegisterHit("Event");
            return RouteEvent();
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private IActionResult RouteEvent()
        {
            try
            {
                //RecipeItem.ResetId();
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                Language lang = this.ReadLanguageCookie(sql);

                var knightRidersCollection = CrossoutDataService.Instance.KnightRidersCollection;
                var statusModel = db.SelectStatus();

                KnightRidersModel knightRidersModel = new KnightRidersModel();

                List<int> containedItemIDs = new List<int>();

                foreach (var item in knightRidersCollection.EventItems)
                {
                    foreach (var ingredient in item.Ingredients)
                    {
                        if (!containedItemIDs.Contains(ingredient.Id))
                        {
                            containedItemIDs.Add(ingredient.Id);
                        }
                    }
                    if (item.Id != null)
                    {
                        containedItemIDs.Add((int)item.Id);
                    }
                }
                knightRidersModel.ContainedItems = db.SelectListOfItems(containedItemIDs, lang.Id);
                foreach (var item in knightRidersModel.ContainedItems)
                {
                    item.Value.SetImageExists(pathProvider);
                }

                foreach (var item in knightRidersCollection.EventItems)
                {
                    decimal sellSum = 0;
                    decimal buySum = 0;
                    foreach (var ingredient in item.Ingredients)
                    {
                        ingredient.Name = knightRidersModel.ContainedItems[ingredient.Id].AvailableName;
                        ingredient.SellPrice = knightRidersModel.ContainedItems[ingredient.Id].SellPrice;
                        ingredient.BuyPrice = knightRidersModel.ContainedItems[ingredient.Id].BuyPrice;
                        ingredient.FormatSellPrice = PriceFormatter.FormatPrice(ingredient.SellPrice);
                        ingredient.FormatBuyPrice = PriceFormatter.FormatPrice(ingredient.BuyPrice);
                        sellSum += ingredient.SellPrice * ingredient.Amount / knightRidersModel.ContainedItems[ingredient.Id].Amount;
                        buySum += ingredient.BuyPrice * ingredient.Amount / knightRidersModel.ContainedItems[ingredient.Id].Amount;
                    }

                    if (item.Id != null)
                    {
                        item.Name = knightRidersModel.ContainedItems[(int)item.Id].AvailableName;
                        item.SellPrice = knightRidersModel.ContainedItems[(int)item.Id].SellPrice;
                        item.BuyPrice = knightRidersModel.ContainedItems[(int)item.Id].BuyPrice;
                        item.FormatSellPrice = PriceFormatter.FormatPrice(item.SellPrice);
                        item.FormatBuyPrice = PriceFormatter.FormatPrice(item.BuyPrice);
                    }

                    item.FormatSellSum = PriceFormatter.FormatPrice(sellSum);
                    item.FormatBuySum = PriceFormatter.FormatPrice(buySum);
                    item.TotalSellSum = sellSum;
                    item.TotalBuySum = buySum;
                    item.FormatTotalSellSum = PriceFormatter.FormatPrice(sellSum);
                    item.FormatTotalBuySum = PriceFormatter.FormatPrice(buySum);
                }
                knightRidersModel.EventItems = knightRidersCollection.EventItems;

                knightRidersModel.Status = statusModel;

                return View("event", knightRidersModel);
            }
            catch
            {
                return Redirect("/");
            }
        }
    }
}