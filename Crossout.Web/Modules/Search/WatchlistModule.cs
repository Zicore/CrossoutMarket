using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Models.General;
using Crossout.Web.Services;
using Nancy;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Modules.Search
{
    public class WatchlistModule : NancyModule
    {
        public WatchlistModule()
        {
            Get["/watchlist/(?<ids>.*)"] = x =>
            {
                return RouteCompare(x);
            };
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private dynamic RouteCompare(dynamic items)
        {
            var result = new List<int>();
            var idsString = (string)items.ids;

            var ids = idsString.Split(',');

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
                    itemList.Add(itemModel.Item);
                }
                var watchlist = new WatchlistModel();
                watchlist.Items = itemList;

                var statusModel = db.SelectStatus();
                watchlist.Status = statusModel;

                return View["watchlist", watchlist];
            }
            catch
            {
                return Response.AsRedirect("/");
            }
        }
    }
}
