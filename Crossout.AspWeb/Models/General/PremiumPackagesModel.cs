using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.AspWeb.Helper;
using Crossout.Data;
using Crossout.Data.PremiumPackages;
using Crossout.AspWeb.Models.View;

namespace Crossout.AspWeb.Models.General
{
    public class PremiumPackagesModel : BaseViewModel, IViewTitle
    {
        public string Title => "Packs";

        public List<PremiumPackage> Packages { get; set; } = new List<PremiumPackage>();

        public Dictionary<int, Item> ContainedItems { get; set; } = new Dictionary<int, Item>();

        public StatusModel Status { get; set; } = new StatusModel();
    }
}
