using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.Model.Items;
using Crossout.Web;
using Crossout.Web.Models.General;
using Crossout.Web.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.AspWeb.Controllers
{
    public class WatchlistController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public WatchlistController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }
        
        [Route("watchlist/{items}")]
        public IActionResult Watchlist(string items)
        {
            return RouteWatchlist(items);
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private IActionResult RouteWatchlist(string items)
        {
            var result = new List<int>();

            var ids = items.Split(',');

            foreach (var id in ids)
            {
                int foundId;
                if (int.TryParse(id, out foundId))
                {
                    result.Add(foundId);
                }
            }

            try
            {
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemList = new List<Item>();

                foreach (var id in result)
                {
                    var itemModel = db.SelectItem(id, true);
                    itemModel.Item.SetImageExists(pathProvider);
                    itemList.Add(itemModel.Item);
                }
                var watchlist = new WatchlistModel();
                watchlist.Items = itemList;

                var statusModel = db.SelectStatus();
                watchlist.Status = statusModel;

                return View("watchlist", watchlist);
            }
            catch
            {
                return Redirect("/");
            }
        }
    }
}