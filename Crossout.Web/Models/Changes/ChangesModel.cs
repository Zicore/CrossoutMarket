using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Helper;
using Crossout.Web.Models.View;
using Crossout.Web.Models.General;
using Crossout.Web.Models.Items;

namespace Crossout.Web.Models.Changes
{
    public class ChangesModel : IViewTitle
    {
        public string Title => "Changes";

        public Dictionary<int, Item> ContainedItems { get; set; }

        public Dictionary<int, string> AllRarites { get; set; }

        public Dictionary<int, string> AllCategories { get; set; }

        public Dictionary<int, string> AllTypes { get; set; }

        public List<ChangeItem> Changes { get; set; } = new List<ChangeItem>();

        public StatusModel Status { get; set; } = new StatusModel();
    }
}
