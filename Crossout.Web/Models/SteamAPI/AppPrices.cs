using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Data.PremiumPackages;

namespace Crossout.Web.Models.SteamAPI
{
    public class AppPrices
    {
        public int Id;
        public List<Currency> Prices;
    }
}
