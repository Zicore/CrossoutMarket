using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return View("tools", model);
        }
    }
}