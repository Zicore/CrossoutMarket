using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Model.Recipes;
using Crossout.Web.Models;
using Crossout.Web.Services;
using Nancy;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Modules.Search
{
    public class CompareModule : NancyModule
    {
        public CompareModule()
        {
            Get["/compare/(?<ids>.*)"] = x =>
            {
                return RouteCompare(x);
            };
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private dynamic RouteCompare(dynamic items)
        {
            var result = new List<int>();
            foreach (var id in ((string)items.ids).Split(','))
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
                    CrossoutDataService.Instance.AddData(itemModel.Item);
                    if (itemModel.Item.Stats != null)
                    {
                        itemList.Add(itemModel.Item);
                    }
                }
                var itemCol = new ItemCollection();
                itemCol.Items = itemList;

                itemCol.CreateStatList();

                return View["compare", itemCol];
            }
            catch
            {
                return Response.AsRedirect("/");
            }
        }
    }
}
