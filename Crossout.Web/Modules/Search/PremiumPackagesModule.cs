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
    public class PremiumPackagesModule : NancyModule
    {
        public PremiumPackagesModule()
        {
            Get["/packages"] = x =>
            {
                return RoutePackages();
            };
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private dynamic RoutePackages()
        {
            try
            {
                //RecipeItem.ResetId();
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var packagesCollection = CrossoutDataService.Instance.PremiumPackagesCollection;
                var statusModel = db.SelectStatus();

                PremiumPackagesModel packagesModel = new PremiumPackagesModel();

                List<int> itemIDs = new List<int>();

                foreach (var package in packagesCollection.Packages)
                {
                    foreach (var itemID in package.MarketPartIDs)
                    {
                        if (!itemIDs.Contains(itemID))
                        {
                            itemIDs.Add(itemID);
                        }
                    }


                }
                packagesModel.ContainedItems = db.SelectListOfItems(itemIDs);

                foreach (var package in packagesCollection.Packages)
                {
                    decimal sellSum = 0;
                    decimal buySum = 0;
                    foreach(var id in package.MarketPartIDs)
                    {
                        sellSum += packagesModel.ContainedItems[id].SellPrice;
                        buySum += packagesModel.ContainedItems[id].BuyPrice;
                    }

                    package.FormatSellSum = PriceFormatter.FormatPrice(sellSum);
                    package.FormatBuySum = PriceFormatter.FormatPrice(buySum);
                    package.TotalSellSum = sellSum + (package.RawCoins * 100);
                    package.TotalBuySum = buySum + (package.RawCoins * 100);
                    package.FormatTotalSellSum = PriceFormatter.FormatPrice(sellSum + (package.RawCoins * 100));
                    package.FormatTotalBuySum = PriceFormatter.FormatPrice(buySum + (package.RawCoins * 100));

                    if (package.Prices != null)
                    {
                        foreach (var price in package.Prices)
                        {
                            if(price.Initialized)
                            {
                                price.FormatSellPriceDividedByCurrency = PriceFormatter.FormatPrice(package.TotalSellSum / price.Final);
                                price.FormatBuyPriceDividedByCurrency = PriceFormatter.FormatPrice(package.TotalBuySum / price.Final);
                            }
                            else
                            {
                                price.FormatSellPriceDividedByCurrency = PriceFormatter.FormatPrice(0);
                                price.FormatBuyPriceDividedByCurrency = PriceFormatter.FormatPrice(0);
                            }
                        }
                    }
                    else
                    {
                        LoadingDataModel loadingDataModel = new LoadingDataModel() { Referrer = "packages"};
                        return View["loadingdata", loadingDataModel];
                    }
                }
                packagesModel.Packages = packagesCollection.Packages;

                packagesModel.Status = statusModel;

                return View["packages", packagesModel];
            }
            catch
            {
                return Response.AsRedirect("/");
            }
        }
    }
}
