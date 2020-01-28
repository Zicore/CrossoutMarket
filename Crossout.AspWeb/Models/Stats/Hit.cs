using Microsoft.AspNetCore.Mvc.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Stats
{
    public class Hit
    {
        public string ControllerName;
        public string ActionName;
        public string ActionId;
        public string ActionDisplayName;
        public int? IdParameter;
        public string RemoteIP;
        public DateTime HitTimestamp;
    }
}
