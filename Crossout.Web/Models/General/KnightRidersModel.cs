using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Helper;
using Crossout.Data;
using Crossout.Data.KnightRiders;

namespace Crossout.Web.Models.General
{
    public class KnightRidersModel : IViewTitle
    {
        public string Title => "Event";

        public List<EventItem> EventItems { get; set; } = new List<EventItem>();

        public Dictionary<int, Item> ContainedItems { get; set; } = new Dictionary<int, Item>();

        public StatusModel Status { get; set; } = new StatusModel();
    }
}
