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
using Crossout.AspWeb.Models.Drafts.Salvage;
using NLog;
using Crossout.Data;

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
        public IActionResult BadgeExchangeRedirect()
        {
            return Redirect("/tools/badgeexchange");
        }

        [Route("tools/badgeexchange")]
        public IActionResult BadgeExchange()
        {
            try
            {
                this.RegisterHit("BadgeExchange");

                sql.Open(WebSettings.Settings.CreateDescription());
                DataService db = new DataService(sql);
                Language lang = this.ReadLanguageCookie(sql);

                var badgeExchangeModel = new BadgeExchangeModel();
                badgeExchangeModel.BadgeExchangeDeals = db.SelectBadgeExchange(lang.Id);
                foreach (var deal in badgeExchangeModel.BadgeExchangeDeals)
                {
                    deal.RewardItem.SetImageExists(pathProvider);
                }
                badgeExchangeModel.Localizations = db.SelectFrontendLocalizations(lang.Id, "badgeexchange");

                return View("badgeexchange", badgeExchangeModel);
            }
            catch
            {
                return Redirect("/");
            }
        }

        [Route("drafts/sniper")]
        public IActionResult SnipeRedirect()
        {
            return Redirect("/tools/sniper");
        }

        [Route("tools/sniper")]
        public IActionResult Snipe()
        {
            try
            {
                this.RegisterHit("Sniper");

                sql.Open(WebSettings.Settings.CreateDescription());
                DataService db = new DataService(sql);
                Language lang = this.ReadLanguageCookie(sql);

                var snipeModel = new SnipeModel();
                snipeModel.SnipeItems = db.SelectSnipeItems(lang.Id);

                foreach (var item in snipeModel.SnipeItems)
                {
                    item.SetImageExists(pathProvider);
                }
                snipeModel.Localizations = db.SelectFrontendLocalizations(lang.Id, "sniper");

                return View("snipe", snipeModel);
            }
            catch
            {
                return Redirect("/");
            }
        }

        [Route("drafts/salvage")]
        public IActionResult SalvageRedirect()
        {
            return Redirect("/tools/salvage");
        }

        [Route("tools/salvage")]
        public IActionResult Salvage()
        {
            try
            {
                this.RegisterHit("Salvager");

                sql.Open(WebSettings.Settings.CreateDescription());
                DataService db = new DataService(sql);
                Language lang = this.ReadLanguageCookie(sql);

                var salvageModel = new SalvageModel();
                salvageModel.SalvageItems = db.SelectSalvageItems(lang.Id);
                salvageModel.SalvageRewards = db.SelectSalvageRewards(lang.Id);

                foreach (var reward in salvageModel.SalvageRewards)
                {
                    reward.Item.SetImageExists(pathProvider);
                }

                foreach (var item in salvageModel.SalvageItems)
                {
                    item.SalvageRewards = salvageModel.SalvageRewards.FindAll(x => x.RarityNumber == item.RarityNumber);
                }

                foreach (var item in salvageModel.SalvageItems)
                {
                    item.SetImageExists(pathProvider);
                }

                return View("salvage", salvageModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Redirect("/");
            }
        }

    }
}