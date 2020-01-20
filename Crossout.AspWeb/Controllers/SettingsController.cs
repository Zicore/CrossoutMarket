using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Models.General;
using Microsoft.AspNetCore.Mvc;

namespace Crossout.AspWeb.Controllers
{
    public class SettingsController : Controller
    {
        [Route("settings")]
        public IActionResult Settings()
        {
            SettingsModel model = new SettingsModel();
            return View("settings", model);
        }
    }
}