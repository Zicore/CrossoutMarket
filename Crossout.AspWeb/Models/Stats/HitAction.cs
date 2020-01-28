using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Stats
{
    public class HitAction
    {
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string DisplayName { get; set; }
        public double Hits { get; set; }
        public double UniqueHits { get; set; }
        public HashSet<string> UniqueIPs { get; set; }
        public List<HitIdParameter> IdParameters { get; set; }

        public void AddHit(Hit hit)
        {
            Hits++;
            if (!UniqueIPs.Contains(hit.RemoteIP))
            {
                UniqueIPs.Add(hit.RemoteIP);
                UniqueHits++;
            }

            if (hit.IdParameter != null)
            {
                HitIdParameter hitIdParameter = IdParameters.Find(x => x.IdParameter == hit.IdParameter);

                if (hitIdParameter != null)
                {
                    hitIdParameter.AddHit(hit);
                }
                else
                {
                    HitIdParameter newIdParameter = new HitIdParameter
                    {
                        IdParameter = (int)hit.IdParameter
                    };
                    newIdParameter.AddHit(hit);

                    IdParameters.Add(newIdParameter);
                }
            }
        }
    }
}
