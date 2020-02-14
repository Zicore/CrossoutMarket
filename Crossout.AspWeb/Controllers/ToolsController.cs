using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.General;
using Microsoft.AspNetCore.Mvc;

namespace Crossout.AspWeb.Controllers
{
    public class ToolsController : Controller
    {
        [Route("tools")]
        public IActionResult Tools()
        {
            ToolsModel model = new ToolsModel();
            this.RegisterHit("Community Tools");

            return View("tools", model);
        }
    }
}