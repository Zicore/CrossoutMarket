using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Helper;
using Crossout.Data;
using Crossout.Data.PremiumPackages;
using Crossout.Web.Models.View;

namespace Crossout.Web.Models.General
{
    public class PremiumPackagesModel : IViewTitle
    {
        public string Title => "Packs";

        public Dictionary<int, List<PremiumPackage>> Packages { get; set; } = new Dictionary<int, List<PremiumPackage>>();

        public Dictionary<int, Item> ContainedItems { get; set; } = new Dictionary<int, Item>();

        public StatusModel Status { get; set; } = new StatusModel();
    }
}
