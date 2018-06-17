using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.Worker.Models.SteamAPI;
using System.Threading;
using Newtonsoft.Json;

namespace Crossout.Worker.Tasks
{
    public class SteamAPITask : BaseTask
    {
        public SteamAPITask(string key) : base(key) { }

        private static List<string> currencysToGet = new List<string> { "us", "de", "uk", "ru" };
        private static HttpClient client = new HttpClient();
        private static List<int> appIDsToGet = new List<int>();
        private static Dictionary<int, AppPrices> appPricesCollection = new Dictionary<int, AppPrices>();
        private static bool isRunning = false;

        public override async void Workload(SqlConnector sql)
        {
            if (!isRunning)
            {
                isRunning = true;
                string query = "SELECT appid FROM steamprices";
                var dataset = sql.SelectDataSet(query);
                appIDsToGet.Clear();
                foreach (var row in dataset)
                {
                    appIDsToGet.Add((int)row[0]);
                }

                await CollectAppPrices();

                foreach (var app in appPricesCollection)
                {
                    if (app.Value.Prices.Count == 4)
                    {
                        List<Parameter> parameters = new List<Parameter>();
                        parameters.Add(new Parameter { Identifier = "@appid", Value = app.Key });
                        parameters.Add(new Parameter { Identifier = "@priceusd", Value = app.Value.Prices[0].Final });
                        parameters.Add(new Parameter { Identifier = "@priceeur", Value = app.Value.Prices[1].Final });
                        parameters.Add(new Parameter { Identifier = "@pricegbp", Value = app.Value.Prices[2].Final });
                        parameters.Add(new Parameter { Identifier = "@pricerub", Value = app.Value.Prices[3].Final });
                        var result = sql.ExecuteSQL("UPDATE steamprices SET steamprices.priceusd = @priceusd, steamprices.priceeur = @priceeur, steamprices.pricegbp = @pricegbp, steamprices.pricerub = @pricerub WHERE steamprices.appid = @appid", parameters);
                    }
                    else
                    {
                        List<Parameter> parameters = new List<Parameter>();
                        parameters.Add(new Parameter { Identifier = "@appid", Value = app.Key });
                        var result = sql.ExecuteSQL("UPDATE steamprices SET steamprices.priceusd = null, steamprices.priceeur = null, steamprices.pricegbp = null, steamprices.pricerub = null WHERE steamprices.appid = @appid", parameters);
                    }
                }
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} finished!");
                isRunning = false;
            } else
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} already running, skipping.");
            }
        }

        private async Task<AppDetails> GetAppDetailsAsync(int id, string currency)
        {
            AppDetails appDetails = null;

            HttpResponseMessage response = await client.GetAsync("http://store.steampowered.com/api/appdetails?appids=" + id + "&cc=" + currency);
            if (response.IsSuccessStatusCode)
            {
                string rawjson = await response.Content.ReadAsStringAsync();
                rawjson = rawjson.Replace("\"" + id + "\"", "\"game\"");
                appDetails = JsonConvert.DeserializeObject<AppDetails>(rawjson);
                appDetails.id = id;
            }
            return appDetails;
        }

        private async Task CollectAppPrices(CancellationToken token = new CancellationToken())
        {
            appPricesCollection.Clear();
            foreach (var id in appIDsToGet)
            {

                AppPrices appPrices = new AppPrices();
                appPrices.Prices = new List<Currency>();
                appPrices.Id = id;

                foreach (var currencystring in currencysToGet)
                {
                    AppDetails appDetails = new AppDetails();
                    appDetails = await GetAppDetailsAsync(id, currencystring);
                    var priceOverview = appDetails.game.data.price_overview;
                    Currency currency = new Currency();
                    if (priceOverview != null)
                    {
                        currency.SteamCurrencyAbbriviation = currencystring;
                        currency.CurrencyAbbriviation = priceOverview.currency;
                        currency.DiscountPercent = priceOverview.discount_percent;
                        currency.Final = priceOverview.final;
                        currency.Initial = priceOverview.initial;
                        appPrices.Prices.Add(currency);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                }
                appPricesCollection.Add(id, appPrices);

            }
        }
    }
}
