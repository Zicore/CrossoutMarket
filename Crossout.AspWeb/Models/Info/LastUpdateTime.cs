using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.View;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Models.Items;

namespace Crossout.AspWeb.Models.Info
{
    public class LastUpdateTime
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
