using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Web.Models
{
    public class StatusModel
    {
        public int Id { get; set; }
        public DateTime LastUpdate { get; set; }

        public bool BackendRunning => DateTime.Now - LastUpdate < new TimeSpan(0, 0, 10, 0);
    }
}
