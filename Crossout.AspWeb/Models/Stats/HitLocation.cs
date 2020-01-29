using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Stats
{
    public class HitLocation
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ActionId { get; set; }
        public string ActionDisplayName { get; set; }
        public int? IdParameter { get; set; }

        public List<TimedHits> TimedHits { get; set; } = new List<TimedHits>();

        public HitLocation(List<TimeSpan> intervals)
        {
            foreach (var interval in intervals)
            {
                var newTimedHits = new TimedHits(interval);
                newTimedHits.Reset();
                TimedHits.Add(newTimedHits);
            }
        }

        public void AddHit(Hit hit)
        {
            foreach (var timedHit in TimedHits)
            {
                if (timedHit.IsExpired)
                    timedHit.Reset();

                timedHit.AddHit(hit);
            }
        }
    }
}
