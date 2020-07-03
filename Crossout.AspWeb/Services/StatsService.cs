using Crossout.AspWeb.Models.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Services
{
    public class StatsService
    {
        public DateTime ServiceStartTime;
        private List<TimeSpan> packageInterval = new List<TimeSpan> { new TimeSpan(365, 0, 0, 0) };
        public List<HitLocation> HitLocations = new List<HitLocation>();
        public List<CombinedHitLocation> CombinedHitLocations = new List<CombinedHitLocation>();

        private StatsService()
        {
            ServiceStartTime = DateTime.UtcNow;

            CombinedHitLocations.Add(new CombinedHitLocation(packageInterval, "Frontend", "Changes|Home|PremiumPackages|Settings|Stats|Tools|Drafts|Info", ".*", ".*", ServiceStartTime));
            CombinedHitLocations.Add(new CombinedHitLocation(packageInterval, "Items", "Item", "Item", ".*", ServiceStartTime));
            CombinedHitLocations.Add(new CombinedHitLocation(packageInterval, "API", "Api|HtmlExport", ".*", ".*", ServiceStartTime));
        }

        public void AddHit(Hit hit)
        {
            HitLocation matchingLocation = HitLocations.Find(x => x.ActionId == hit.ActionId && x.IdParameter == hit.IdParameter);
            if (matchingLocation != null)
            {
                matchingLocation.AddHit(hit);
                foreach (var combinedHitLocation in CombinedHitLocations)
                {
                    combinedHitLocation.CheckLocation(matchingLocation);
                }
            }
            else
            {
                var newHitLocation = new HitLocation(packageInterval, ServiceStartTime);
                newHitLocation.ActionDisplayName = hit.ActionDisplayName;
                newHitLocation.ActionId = hit.ActionId;
                newHitLocation.ActionName = hit.ActionName;
                newHitLocation.ControllerName = hit.ControllerName;
                newHitLocation.IdParameter = hit.IdParameter;
                newHitLocation.AddHit(hit);
                HitLocations.Add(newHitLocation);

                foreach (var combinedHitLocation in CombinedHitLocations)
                {
                    combinedHitLocation.CheckLocation(newHitLocation);
                }
            }
        }


        public static void Initialize()
        {
            _instance = new StatsService();
        }

        private static StatsService _instance;

        public static StatsService Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
