using Crossout.AspWeb.Models.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Services
{
    public class StatsService
    {
        private List<Hit> unpackagedHits;
        private Dictionary<string, string> actionIdByName;
        private DateTime serviceStartTime;
        private DateTime lastPackageTime;
        private Dictionary<TimeSpan, List<HitTimespan>> hitPackageTimeSpans;
        private List<TimeSpan> packageInterval = new List<TimeSpan> { new TimeSpan(0, 1, 0), new TimeSpan(0, 60, 0), new TimeSpan(1, 0, 0), new TimeSpan(7, 0, 0) };
        public List<HitTimespan> LatestHitTimeSpans { get; private set; }

        private StatsService()
        {
            serviceStartTime = DateTime.UtcNow;
            lastPackageTime = DateTime.UtcNow;
            unpackagedHits = new List<Hit>();
            hitPackageTimeSpans = new Dictionary<TimeSpan, List<HitTimespan>>();
            LatestHitTimeSpans = new List<HitTimespan>();
        }

        public void AddHit(Hit hit)
        {
            unpackagedHits.Add(hit);

            TimeSpan timeSpanSinceLastPackage = DateTime.UtcNow.Subtract(lastPackageTime);
            if (timeSpanSinceLastPackage > packageInterval.First())
            {
                PackageHits();
            }
        }

        private void PackageHits()
        {
            int i = 0;
            foreach (var interval in packageInterval)
            {
                HitTimespan package = new HitTimespan(interval);
                if (i == 0)
                {
                    foreach (var hit in unpackagedHits)
                    {
                        package.AddHit(hit);
                    }
                }
                else
                {
                    HitTimespan latestHitTimespan = LatestHitTimeSpans.Find(x => x.Interval == interval);

                    if (latestHitTimespan.IsPackageExpired)
                    {
                        if (hitPackageTimeSpans.ContainsKey(interval))
                        {
                            hitPackageTimeSpans[interval].Add(latestHitTimespan);
                        }
                        else
                        {
                            hitPackageTimeSpans.Add(interval, new List<HitTimespan> { latestHitTimespan });
                        }
                        TimeSpan previousInterval = packageInterval[i - 1];
                        List<HitTimespan> previousSpans = hitPackageTimeSpans[previousInterval];
                        package.AddFromPreviousSpans(previousSpans);
                        hitPackageTimeSpans[previousInterval].Clear();
                    }
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
