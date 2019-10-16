using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.Data.PremiumPackages;
using Crossout.Model.Formatter;
using Crossout.Web;
using Crossout.Web.Models.General;
using Crossout.Web.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.AspWeb.Controllers
{
    public class PremiumPackagesController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public PremiumPackagesController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        [Route("packs")]
        public IActionResult Packages()
        {
            return RoutePackages();
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private IActionResult RoutePackages()
        {
            try
            {
                //RecipeItem.ResetId();
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var packagesCollection = CrossoutDataService.Instance.PremiumPackagesCollection;
                var statusModel = db.SelectStatus();
                var appPrices = db.SelectAllSteamPrices();

                PremiumPackagesModel packagesModel = new PremiumPackagesModel();

                List<int> itemIDs = new List<int>();

                //Load contained items
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
                foreach (var item in packagesModel.ContainedItems)
                {
                    item.Value.SetImageExists(pathProvider);
                }

                //Calc prizes
                foreach (var package in packagesCollection.Packages)
                {
                    package.Prices = appPrices.Find(x => x.Id == package.SteamAppID).Prices;
                    decimal sellSum = 0;
                    decimal buySum = 0;
                    foreach (var id in package.MarketPartIDs)
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

                    foreach (var price in package.Prices)
                    {
                        if (price != null && price.Final != 0)
                        {
                            price.FormatFinal = PriceFormatter.FormatPrice(price.Final);
                            price.FormatSellPriceDividedByCurrency = PriceFormatter.FormatPrice(package.TotalSellSum / ((decimal)price.Final / 100));
                            price.FormatBuyPriceDividedByCurrency = PriceFormatter.FormatPrice(package.TotalBuySum / ((decimal)price.Final / 100));
                        }
                    }
                }

                //Add all possible categories to dict
                var listOfCategories = new List<int>();
                listOfCategories.Clear();
                listOfCategories.Add(1);
                listOfCategories.Add(99);
                foreach (var package in packagesCollection.Packages)
                {
                    if (!listOfCategories.Contains(package.Category) && package.Category != 0)
                    {
                        listOfCategories.Add(package.Category);
                    }
                }
                listOfCategories.Sort();
                foreach (var category in listOfCategories)
                {
                    packagesModel.Packages.Add(category, new List<PremiumPackage>());
                }

                //Categorize
                foreach (var package in packagesCollection.Packages)
                {

                    if (package.Prices.Any(x => x.Final != 0))
                    {
                        if (package.Category == 0)
                        {
                            package.Category = 1;
                        }
                    }
                    else
                    {
                        package.Category = 99;
                    }

                    packagesModel.Packages[package.Category].Add(package);
                }

                packagesModel.Status = statusModel;

                return View("packages", packagesModel);
            }
            catch
            {
                return Redirect("/");
            }
        }
    }
}