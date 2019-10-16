using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web;
using Crossout.Web.Models.Items;
using Crossout.Web.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.AspWeb.Controllers
{
    public class ChangesController : Controller
    {
        [Route("changes")]
        public IActionResult Changes()
        {
            return RouteItem();
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private IActionResult RouteItem()
        {
            try
            {
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemModel = new ItemModel();
                var changesModel = db.SelectChanges();
                var statusModel = db.SelectStatus();

                changesModel.Status = statusModel;

                // Note: Only Ids and Names get filled in
                var allItems = db.SelectAllActiveItems(false);
                var allRarites = db.SelectAllRarities();
                allRarites.Add(0, "None");
                var allCategories = db.SelectAllCategories();
                allCategories.Add(0, "None");
                var allTypes = db.SelectAllTypes();
                var itemDict = new Dictionary<int, Item>();

                foreach (var item in allItems)
                {
                    itemDict.Add(item.Id, item);
                }
                changesModel.ContainedItems = itemDict;
                changesModel.AllRarites = allRarites;
                changesModel.AllCategories = allCategories;
                changesModel.AllTypes = allTypes;

                return View("changes", changesModel);
            }
            catch
            {
                return Redirect("/");
            }
        }
    }
}