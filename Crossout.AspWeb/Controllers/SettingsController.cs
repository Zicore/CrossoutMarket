using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crossout.AspWeb.Controllers
{
    public class SettingsController : Controller
    {
        [Route("settings")]
        public IActionResult Settings()
        {
            SettingsModel model = new SettingsModel();

            this.RegisterHit("Settings");
            return View("settings", model);
        }
    }
}