using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.Stats;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.Model.Items;

namespace Crossout.AspWeb.Controllers
{
    public class StatsController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public StatsController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("stats")]
        public IActionResult Stats()
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            List<Item> allItems = db.SelectAllActiveItems(false);
            foreach (var item in allItems)
            {
                item.SetImageExists(pathProvider);
            }

            this.RegisterHit("Stats");
            var model = new StatsModel();
            model.ServiceStart = StatsService.Instance.ServiceStartTime;
            model.AllItemsById = allItems.ToDictionary(x => x.Id, x => x);
            model.HitLocations = StatsService.Instance.HitLocations;
            model.CombinedHitLocations = StatsService.Instance.CombinedHitLocations;
            return View("stats", model);
        }
    }
}
