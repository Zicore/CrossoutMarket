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
            Get["/compare/(?<others>.*)"] = Render;
        }

        private Response Render(dynamic parameters)
        {
            var result = new List<int>();
            foreach (var other in ((string)parameters.others).Split('|'))
            {
                try
                {
                    result.Add(Convert.ToInt32((other)));
                }
                catch
                {
                    return Response.AsRedirect("/");
                }

            }
            return RouteCompare(result);
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private dynamic RouteCompare(List<int> ids)
        {
            try
            {

                RecipeItem.ResetId();
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemList = new List<Item>();

                foreach (var id in ids)
                {
                    var itemModel = db.SelectItem(id, true);
                    CrossoutDataService.Instance.AddData(itemModel.Item);
                    itemList.Add(itemModel.Item);
                }
                var itemCol = new ItemCollection();
                itemCol.Items = itemList;

                return View["compare", itemCol];
            }
            catch
            {
                return Response.AsRedirect("/");
            }
        }
    }
}
