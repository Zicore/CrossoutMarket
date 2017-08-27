using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Helper;

namespace Crossout.Web.Models.General
{
    public class WatchlistModel : IViewTitle
    {
        public string Title => "Watchlist";

        public List<Item> Items { get; set; } = new List<Item>();

        public StatusModel Status { get; set; } = new StatusModel();
    }
}
