using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.AspWeb.Helper;
using Crossout.Data;
using Crossout.Data.KnightRiders;
using Crossout.AspWeb.Models.View;

namespace Crossout.AspWeb.Models.General
{
    public class KnightRidersModel : BaseViewModel, IViewTitle
    {
        public string Title => "Event";

        public List<EventItem> EventItems { get; set; } = new List<EventItem>();

        public Dictionary<int, Item> ContainedItems { get; set; } = new Dictionary<int, Item>();

        public StatusModel Status { get; set; } = new StatusModel();
    }
}
