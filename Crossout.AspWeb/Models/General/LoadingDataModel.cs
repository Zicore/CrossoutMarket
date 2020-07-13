using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.View;

namespace Crossout.AspWeb.Models.General
{
    public class LoadingDataModel : BaseViewModel, IViewTitle
    {
        public string Title => "Loading Data";

        public string Referrer;
    }
}
