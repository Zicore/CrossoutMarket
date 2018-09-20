using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Model.Recipes;
using Crossout.Web.Models.General;
using Crossout.Web.Services;
using Crossout.Model.Formatter;
using Nancy;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Modules.Search
{
    public class EventModule : NancyModule
    {
        public EventModule()
        {
            Get["/event"] = x =>
            {
                return RouteEvent();
            };
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private dynamic RouteEvent()
        {
            try
            {
                //RecipeItem.ResetId();
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

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
                knightRidersModel.ContainedItems = db.SelectListOfItems(containedItemIDs);

                foreach (var item in knightRidersCollection.EventItems)
                {
                    decimal sellSum = 0;
                    decimal buySum = 0;
                    foreach(var ingredient in item.Ingredients)
                    {
                        ingredient.Name = knightRidersModel.ContainedItems[ingredient.Id].Name;
                        ingredient.SellPrice = knightRidersModel.ContainedItems[ingredient.Id].SellPrice;
                        ingredient.BuyPrice = knightRidersModel.ContainedItems[ingredient.Id].BuyPrice;
                        ingredient.FormatSellPrice = PriceFormatter.FormatPrice(ingredient.SellPrice);
                        ingredient.FormatBuyPrice = PriceFormatter.FormatPrice(ingredient.BuyPrice);
                        sellSum += ingredient.SellPrice * ingredient.Amount / knightRidersModel.ContainedItems[ingredient.Id].Amount;
                        buySum += ingredient.BuyPrice * ingredient.Amount / knightRidersModel.ContainedItems[ingredient.Id].Amount;
                    }

                    if (item.Id != null)
                    {
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

                return View["event", knightRidersModel];
            }
            catch
            {
                return Response.AsRedirect("/");
            }
        }
    }
}
