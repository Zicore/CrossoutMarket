using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Stats
{
    public class HitIdParameter
    {
        public int IdParameter { get; set; }
        public double Hits { get; set; }
        public double UniqueHits { get; set; }
        public HashSet<string> UniqueIPs { get; set; }

        public void AddHit(Hit hit)
        {
            Hits++;
            if (!UniqueIPs.Contains(hit.RemoteIP))
            {
                UniqueIPs.Add(hit.RemoteIP);
                UniqueHits++;
            }
        }
    }
}
