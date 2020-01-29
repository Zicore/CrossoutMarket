using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Stats
{
    public class CombinedHitLocation
    {
        public string DisplayName;
        private Regex controllerNameRegex;
        private Regex actionNameRegex;
        private Regex idParameterRegex;
        private List<TimedHits> timedHits = new List<TimedHits>();
        public List<TimedHits> TimedHits { get { return GetTotalHits(); } }

        public CombinedHitLocation(List<TimeSpan> intervals, string displayName, string controllerNamePattern, string actionNamePattern, string idParameterPattern, DateTime startTimestamp)
        {
            foreach (var interval in intervals)
            {
                var newTimedHits = new TimedHits(interval, startTimestamp);
                newTimedHits.Reset();
                timedHits.Add(newTimedHits);
            }
            DisplayName = displayName;
            controllerNameRegex = new Regex(controllerNamePattern);
            actionNameRegex = new Regex(actionNamePattern);
            idParameterRegex = new Regex(idParameterPattern);
        }

        public List<HitLocation> Locations { get; set; } = new List<HitLocation>();

        public void CheckLocation(HitLocation location)
        {
            if (!controllerNameRegex.IsMatch(location.ControllerName))
                return;
            if (!actionNameRegex.IsMatch(location.ActionName))
                return;
            if (!idParameterRegex.IsMatch(location.IdParameter.ToString()))
                return;

            bool exists = Locations.Exists(x => x.ActionId == location.ActionId && x.IdParameter == location.IdParameter);

            if (!exists)
                Locations.Add(location);
        }

        public List<TimedHits> GetTotalHits()
        {
            foreach (var hits in timedHits)
            {
                hits.Reset(false);
            }
            for (int i = 0; i < timedHits.Count; i++)
            {
                foreach (var location in Locations)
                {
                    timedHits[i].Hits += location.TimedHits[i].Hits;
                    timedHits[i].UniqueHitHashSet.UnionWith(location.TimedHits[i].UniqueHitHashSet);
                }
            }
            return timedHits;
        }
    }
}
