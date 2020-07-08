using Crossout.AspWeb.Models.View;
using Crossout.AspWeb.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Drafts.BadgeExchange
{
    public class BadgeExchangeModel : IViewTitle
    {
        public string Title => "Badge Exchange";

        public List<FrontendLocalizationPoco> Localizations = new List<FrontendLocalizationPoco>();

        public List<BadgeExchangeDeal> BadgeExchangeDeals = new List<BadgeExchangeDeal>();

        public bool IsUpToDate()
        {
            var daysOffset = (7 + (DateTime.UtcNow.DayOfWeek - DayOfWeek.Monday)) % 7;
            var monday = DateTime.UtcNow.AddDays(-1 * daysOffset).Date;
            foreach (var deal in BadgeExchangeDeals)
            {
                if (deal.Active && deal.LastBeginActive.CompareTo(monday) >= 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
