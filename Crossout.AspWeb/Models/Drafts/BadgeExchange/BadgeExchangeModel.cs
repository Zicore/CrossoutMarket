using Crossout.AspWeb.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Drafts.BadgeExchange
{
    public class BadgeExchangeModel : IViewTitle
    {
        public string Title => "Badge Exchange";

        public List<BadgeExchangeDeal> BadgeExchangeDeals = new List<BadgeExchangeDeal>();

        public bool IsUpToDate()
        {
            var monday = DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek + (int)DayOfWeek.Monday).Date;
            foreach (var deal in BadgeExchangeDeals)
            {
                if (deal.Active && deal.LastBeginActive.CompareTo(monday) < 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
