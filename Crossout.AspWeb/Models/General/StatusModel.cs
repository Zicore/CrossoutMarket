using System;

namespace Crossout.AspWeb.Models.General
{
    public class StatusModel
    {
        public int Id { get; set; }
        public DateTime LastUpdate { get; set; }

        public bool BackendRunning => DateTime.UtcNow - LastUpdate < new TimeSpan(0, 0, 10, 0);
    }
}
