using Crossout.AspWeb.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Drafts
{
    public class DraftsModel : BaseViewModel, IViewTitle
    {
        public string Title => "Drafts";
    }
}
