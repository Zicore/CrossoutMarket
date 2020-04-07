using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Models.Language;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.AspWeb.Controllers
{
    public class SettingsController : Controller
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("settings")]
        public IActionResult Settings()
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            LanguageModel languageModel = db.SelectLanguageModel();

            SettingsModel model = new SettingsModel();
            model.LanguageModel = languageModel;

            this.RegisterHit("Settings");
            return View("settings", model);
        }
    }
}