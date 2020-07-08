using Crossout.AspWeb.Pocos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.View
{
    public class BaseViewModel
    {
        [JsonIgnore]
        public List<FrontendLocalizationPoco> Localizations = new List<FrontendLocalizationPoco>();
    }
}
