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
    public class ToolsController : Controller
    {
        [Route("tools")]
        public IActionResult ToolsRedirect()
        {
            ToolsModel model = new ToolsModel();
            this.RegisterHit("Community Tools (deprecated)");

            return View("toolsredirect", model);
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("links")]
        public IActionResult Tools()
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            Language lang = this.ReadLanguageCookie(sql);

            ToolsModel model = new ToolsModel();
            model.Localizations = db.SelectFrontendLocalizations(lang.Id, "communitylinks");

            this.RegisterHit("Community Links");

            return View("tools", model);
        }
    }
}