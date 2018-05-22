using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Web.Models.SteamAPI;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using Crossout.Data;
using Crossout.Data.PremiumPackages;

namespace Crossout.Web.Services
{
    public class SteamAPIService
    {
        private static HttpClient client = new HttpClient();
        private static List<string> currencysToGet = new List<string> { "us", "de", "uk", "ru" };

        public static List<int> AppIDsToGet = new List<int>();
        public static Dictionary<int, AppPrices> AppPricesCollection { get; set; } = new Dictionary<int, AppPrices>();

        private static async Task<AppDetails> GetAppDetailsAsync(int id, string currency)
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

        public static async Task CollectAppPrices(CancellationToken token = new CancellationToken())
        {
            Dictionary<int, AppPrices> appPricesCollection = new Dictionary<int, AppPrices>();
            foreach (var id in AppIDsToGet)
            {
                AppPrices appPrices = new AppPrices();
                appPrices.Prices = new List<Currency>();
                appPrices.Id = id;
                foreach(var currencystring in currencysToGet)
                {
                    AppDetails appDetails = new AppDetails();
                    appDetails = await GetAppDetailsAsync(id, currencystring);
                    var priceOverview = appDetails.game.data.price_overview;
                    Currency currency = new Currency();
                    currency.CurrencyAbbriviation = priceOverview.currency;
                    currency.DiscountPercent = priceOverview.discount_percent;
                    currency.Final = ((decimal)priceOverview.final / 100);
                    currency.Initial = ((decimal)priceOverview.initial / 100);
                    currency.Initialized = true;
                    appPrices.Prices.Add(currency);
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                }
                appPricesCollection.Add(id, appPrices);
            }
            AppPricesCollection = appPricesCollection;
        }
    }
}
