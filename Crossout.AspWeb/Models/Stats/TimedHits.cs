using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Stats
{
    public class TimedHits
    {
        public DateTime CreationTimestamp { get; set; }
        public TimeSpan Interval { get; set; }
        public bool IsExpired
        {
            get
            {
                return DateTime.UtcNow.Subtract(CreationTimestamp) > Interval;
            }
        }
        public long Hits { get; set; }
        public long UniqueHits { get { return UniqueHitHashSet.Count; } }
        public double HitsPerMinute { get { return Hits / DateTime.UtcNow.Subtract(CreationTimestamp).TotalMinutes; } }
        public string FormatHitsPerMinute { get { return HitsPerMinute.ToString("0.00"); } }
        public double HitsPerHour { get { return Hits / DateTime.UtcNow.Subtract(CreationTimestamp).TotalHours; } }
        public string FormatHitsPerHour { get { return HitsPerHour.ToString("0.00"); } }
        public double HitsPerDay { get { return Hits / DateTime.UtcNow.Subtract(CreationTimestamp).TotalDays; } }
        public string FormatHitsPerDay { get { return HitsPerDay.ToString("0.00"); } }
        public double HitsPerWeek { get { return Hits / (DateTime.UtcNow.Subtract(CreationTimestamp).TotalDays / 7); } }
        public string FormatHitsPerWeek { get { return HitsPerWeek.ToString("0.00"); } }
        public HashSet<int> UniqueHitHashSet { get; set; } = new HashSet<int>();

        public TimedHits(TimeSpan interval)
        {
            Interval = interval;
        }

        public void AddHit(Hit hit)
        {
            Hits++;
            if (!UniqueHitHashSet.Contains(hit.IPHash))
                UniqueHitHashSet.Add(hit.IPHash);
        }

        public void Reset(bool resetTimestamp = true)
        {
            if (resetTimestamp)
                CreationTimestamp = DateTime.UtcNow;
            Hits = 0;
            UniqueHitHashSet.Clear();
        }
    }
}
