using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Model.Formatter
{
    public class PriceFormatter
    {
        public static string FormatPrice(int price)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", price / 100m);
        }

        public static string FormatPrice(decimal price)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", price / 100m);
        }
    }
}
