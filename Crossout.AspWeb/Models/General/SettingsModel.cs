using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.Language;
using Crossout.AspWeb.Models.View;

namespace Crossout.AspWeb.Models.General
{
    public class SettingsModel : BaseViewModel, IViewTitle
    {
        public LanguageModel LanguageModel { get; set; }

        public string Title => "Settings";
    }
}
