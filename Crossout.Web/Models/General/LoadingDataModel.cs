using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Web.Helper;

namespace Crossout.Web.Models.General
{
    public class LoadingDataModel : IViewTitle
    {
        public string Title => "Loading Data";

        public string Referrer;
    }
}
