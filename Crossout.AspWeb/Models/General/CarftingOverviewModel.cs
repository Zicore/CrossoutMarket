using Crossout.Model.Items;
using Crossout.Web.Helper;
using Crossout.Web.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Web.Models.View;

namespace Crossout.Web.Models.General
{
    public class CraftingOverviewModel : IViewTitle
    {
        public List<Item> Items { get; set; }
        public FilterModel FilterModel { get; set; } = new FilterModel();
        public StatusModel Status { get; set; } = new StatusModel();

        public string Title => "Crafting";
    }
}
