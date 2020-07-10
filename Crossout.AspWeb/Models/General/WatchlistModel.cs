using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.View;

namespace Crossout.AspWeb.Models.General
{
    public class WatchlistModel : BaseViewModel, IViewTitle
    {
        public string Title => "Watchlist";

        public List<Item> Items { get; set; } = new List<Item>();

        public StatusModel Status { get; set; } = new StatusModel();
    }
}
