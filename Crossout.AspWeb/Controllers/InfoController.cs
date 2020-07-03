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
using Crossout.AspWeb.Models.Language;
using Crossout.AspWeb.Models.Info;
using MySql.Data.MySqlClient.Properties;

namespace Crossout.AspWeb.Controllers
{
    public class InfoController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public InfoController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        [Route("info")]
        public IActionResult Info()
        {
            Language lang = this.ReadLanguageCookie(sql);
            this.RegisterHit("Info");
            return RouteInfo(lang.Id);
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private IActionResult RouteInfo(int language)
        {
            try
            {
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var infoModel = new InfoModel();
                infoModel.LastUpdateTimes.Add(db.SelectLastUpdate());


                var resourceService = ResourceService.Instance;
                infoModel.Contributors = resourceService.ContributorCollection.Contributors;
                infoModel.UpdateNotes = resourceService.UpdateNoteCollection.UpdateNotes;
                infoModel.UpdateNotes.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));

                infoModel.StatusModel = db.SelectStatus();

                var changesModel = db.SelectChanges();

                changesModel.Status = infoModel.StatusModel;

                // Note: Only Ids and Names get filled in
                var allItems = db.SelectAllActiveItems(language, false);
                var allRarites = db.SelectAllRarities();
                allRarites.Add(0, "None");
                var allCategories = db.SelectAllCategories();
                allCategories.Add(0, "None");
                var allTypes = db.SelectAllTypes();
                var itemDict = new Dictionary<int, Item>();

                foreach (var item in allItems)
                {
                    item.SetImageExists(pathProvider);
                    itemDict.Add(item.Id, item);
                }
                changesModel.ContainedItems = itemDict;
                changesModel.AllRarites = allRarites;
                changesModel.AllCategories = allCategories;
                changesModel.AllTypes = allTypes;
                changesModel.IsSingleItem = false;

                infoModel.ChangesModel = changesModel;


                return View("info", infoModel);
            }
            catch
            {
                return Redirect("/");
            }
        }
    }
}