using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Models;
using Nancy;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Modules.Search
{
    public class ItemModule : NancyModule
    {
        public ItemModule()
        {
            Get["/item/{id:int}"] = x =>
            {
                var id = (int)x.id;
                return RouteItem(id);
            };
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private dynamic RouteItem(int id)
        {
            ItemModel itemModel = new ItemModel ();

            sql.Open(WebSettings.Settings.CreateDescription());

            var parmeter = new List<Parameter>();
            parmeter.Add(new Parameter {Identifier = "id", Value = id});

            string query = SearchModule.BuildSearchQuery(false, false, false, true, false,false);

            var ds = sql.SelectDataSet(query, parmeter);
            
            if (ds != null && ds.Count > 0)
            {
                var item = Item.Create(ds[0]);
                itemModel.Item = item;

                return View["item", itemModel];
            }
            return Response.AsRedirect("/");
        }
    }
}
