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

namespace Crossout.AspWeb.Models.Changes
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
