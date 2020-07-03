using Crossout.AspWeb.Models.View;
using Crossout.AspWeb.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Drafts.Salvage
{
    public class SalvageModel : IViewTitle
    {
        public string Title => "Salvager";

        public List<ItemPoco> SalvageItems { get; set; } = new List<ItemPoco>();

        public List<SalvageRewardPoco> SalvageRewards { get; set; } = new List<SalvageRewardPoco>();
    }
}
