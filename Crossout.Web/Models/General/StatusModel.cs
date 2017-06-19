using System;

namespace Crossout.Web.Models.General
{
    public class StatusModel
    {
        public int Id { get; set; }
        public DateTime LastUpdate { get; set; }

        public bool BackendRunning => DateTime.Now - LastUpdate < new TimeSpan(0, 0, 10, 0);
    }
}
