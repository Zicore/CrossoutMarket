using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Stats
{
    public class HitTimespan
    {
        public HitTimespan(TimeSpan interval)
        {
            Interval = interval;
            PackageCreationTime = DateTime.UtcNow;
            HitCount = 0;
            UniqueIPs = new HashSet<string>();
            Actions = new List<HitAction>();
        }

        public TimeSpan Interval { get; set; }
        public DateTime PackageCreationTime { get; set; }
        public bool IsPackageExpired { get => DateTime.UtcNow.Subtract(PackageCreationTime) > Interval; }
        public double HitCount { get; set; }
        public double UniqueHits { get { return UniqueIPs.Count; } }
        public HashSet<string> UniqueIPs { get; set; }
        public List<HitAction> Actions { get; set; }

        public void AddHit(Hit hit)
        {
            HitCount++;
            if (!UniqueIPs.Contains(hit.RemoteIP))
            {
                UniqueIPs.Add(hit.RemoteIP);
            }

            HitAction action = Actions.Find(x => x.ActionId == hit.ActionId);

            if (action != null)
            {
                action.AddHit(hit);
            }
            else
            {
                HitAction newAction = new HitAction
                {
                    ActionId = hit.ActionId,
                    ActionName = hit.ActionName,
                    ControllerName = hit.ControllerName
                };
                newAction.AddHit(hit);

                Actions.Add(newAction);
            }
        }

        public void AddFromPreviousSpans(List<HitTimespan> hitTimespans)
        {
            foreach(var hitTimespan in hitTimespans)
            {
                HitCount += hitTimespan.HitCount;
                UniqueIPs.UnionWith(hitTimespan.UniqueIPs);
                
            }
        }
    }
}
