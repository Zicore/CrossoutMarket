using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Helper;
using Crossout.Data;
using Crossout.Data.PremiumPackages;

namespace Crossout.Web.Models.General
{
    public class PremiumPackagesModel : IViewTitle
    {
        public string Title => "Packs";

        public List<PremiumPackage> Packages { get; set; } = new List<PremiumPackage>();

        public Dictionary<int, Item> ContainedItems { get; set; } = new Dictionary<int, Item>();

        public StatusModel Status { get; set; } = new StatusModel();
    }
}
