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
using Crossout.AspWeb.Models.Drafts;
using Crossout.AspWeb.Models.Drafts.BadgeExchange;
using Crossout.AspWeb.Models.Drafts.Snipe;

namespace Crossout.AspWeb.Controllers
{
    public class DraftsController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public DraftsController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        [Route("drafts")]
        public IActionResult Drafts()
        {
            this.RegisterHit("Drafts");
            var draftsModel = new DraftsModel();
            return View("drafts", draftsModel);
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("drafts/badgeexchange")]
        public IActionResult BadgeExchange()
        {
            try
            {
                this.RegisterHit("Drafts/BadgeExchange");

                sql.Open(WebSettings.Settings.CreateDescription());
                DataService db = new DataService(sql);
                Language lang = this.ReadLanguageCookie(sql);

                var badgeExchangeModel = new BadgeExchangeModel();
                badgeExchangeModel.BadgeExchangeDeals = db.SelectBadgeExchange(lang.Id);
                foreach (var deal in badgeExchangeModel.BadgeExchangeDeals)
                {
                    deal.RewardItem.SetImageExists(pathProvider);
                }

                return View("badgeexchange", badgeExchangeModel);
            }
            catch
            {
                return Redirect("/");
            }
        }

        [Route("drafts/sniper")]
        public IActionResult Snipe()
        {
            try
            {
                this.RegisterHit("Drafts/Sniper");

                sql.Open(WebSettings.Settings.CreateDescription());
                DataService db = new DataService(sql);
                Language lang = this.ReadLanguageCookie(sql);

                var snipeModel = new SnipeModel();
                snipeModel.SnipeItems = db.SelectSnipeItems(lang.Id);

                foreach (var item in snipeModel.SnipeItems)
                {
                    item.SetImageExists(pathProvider);
                }

                return View("snipe", snipeModel);
            }
            catch
            {
                return Redirect("/");
            }
        }

    }
}