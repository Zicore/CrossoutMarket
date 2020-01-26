using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.WorkerCore.Models.SteamAPI
{
    public class Currency
    {
        public string CurrencyAbbriviation;
        public string SteamCurrencyAbbriviation;
        public int Initial;
        public int Final;
        public int DiscountPercent;
        public string FormatSellPriceDividedByCurrency;
        public string FormatBuyPriceDividedByCurrency;
    }
}
