using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.Model.Items;
using Crossout.Web;
using Crossout.Web.Models.Items;
using Crossout.Web.Services;
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

        private IActionResult RouteCompare(dynamic items)
        {
            var result = new List<int>();

            var ids = items.Split(',');

            foreach (var id in ids)
            {
                int foundId;
                if (int.TryParse(id, out foundId))
                {
                    if (foundId > 0)
                    {
                        result.Add(foundId);
                    }
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
                    CrossoutDataService.Instance.AddData(itemModel.Item);
                    itemList.Add(itemModel.Item);
                }
                var itemCol = new ItemCollection();
                itemCol.Items = itemList;

                itemCol.CreateStatList();
                itemCol.AllItems = db.SelectAllActiveItems();

                return View("compare", itemCol);
            }
            catch
            {
                return Redirect("/");
            }
        }
    }
}