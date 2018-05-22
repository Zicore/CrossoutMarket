using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Data.PremiumPackages
{
    public class Currency
    {
        public bool Initialized;
        public string CurrencyAbbriviation;
        public decimal Initial;
        public decimal Final;
        public int DiscountPercent;
        public string FormatSellPriceDividedByCurrency;
        public string FormatBuyPriceDividedByCurrency;
    }

    public class PremiumPackage
    {
        public string Key;
        public int SteamAppID;
        public string Name;
        public string Description;
        public int[] MarketPartIDs;
        public string FormatSellSum;
        public string FormatBuySum;
        public decimal TotalSellSum;
        public decimal TotalBuySum;
        public string FormatTotalSellSum;
        public string FormatTotalBuySum;
        public int RawCoins;
        public List<Currency> Prices;
    }
}
