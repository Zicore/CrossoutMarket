using Crossout.AspWeb.Models.View;
using Crossout.AspWeb.Services;
using Crossout.Model.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Stats
{
    public class StatsModel : IViewTitle
    {
        public DateTime ServiceStart { get; set; }
        public TimeSpan TimeSpanSinceStart
        {
            get
            {
                return DateTime.UtcNow.Subtract(ServiceStart);
            }
        }
        public string FormatTimeSpanSinceStart
        {
            get
            {
                string formatString = "";
                if (TimeSpanSinceStart.Days > 0)
                    formatString += $"{TimeSpanSinceStart.Days} days ";
                if (TimeSpanSinceStart.Hours > 0)
                    formatString += $"{TimeSpanSinceStart.Hours} hours ";
                if (TimeSpanSinceStart.Minutes > 0)
                    formatString += $"{TimeSpanSinceStart.Minutes} minutes ";
                if (TimeSpanSinceStart.Seconds > 0)
                    formatString += $"{TimeSpanSinceStart.Seconds} seconds ";
                formatString += "since service start";
                return formatString;
            }
        }
        public List<HitLocation> HitLocations { get; set; } = new List<HitLocation>();
        public List<CombinedHitLocation> CombinedHitLocations { get; set; } = new List<CombinedHitLocation>();
        public Dictionary<int, Item> AllItemsById { get; set; } = new Dictionary<int, Item>();

        public string Title => "Stats";
    }
}
