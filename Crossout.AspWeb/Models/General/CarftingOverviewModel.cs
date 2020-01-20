using Crossout.Model.Items;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.AspWeb.Models.View;

namespace Crossout.AspWeb.Models.General
{
    public class CraftingOverviewModel : IViewTitle
    {
        public List<Item> Items { get; set; }
        public FilterModel FilterModel { get; set; } = new FilterModel();
        public StatusModel Status { get; set; } = new StatusModel();

        public string Title => "Crafting";
    }
}
