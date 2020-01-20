using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.Model.Items;
using Crossout.AspWeb;
using Crossout.AspWeb.Models.Items;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.AspWeb.Controllers
{
    public class CompareController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public CompareController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        [Route("compare/{items}")]
        public IActionResult Compare(string items)
        {
            return RouteCompare(items);
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private IActionResult RouteCompare(string items)
        {
            try
            {
                var itemCol = new ItemCollection();
                itemCol.ItemList = items;

                return View("compare", itemCol);
            }
            catch
            {
                return Redirect("/");
            }
        }
    }
}