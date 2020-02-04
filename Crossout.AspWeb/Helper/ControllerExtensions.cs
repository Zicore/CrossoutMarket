using Crossout.AspWeb.Models.Language;
using Crossout.AspWeb.Models.Stats;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.AspWeb.Helper
{
    public static class ControllerExtensions
    {
        public static HashSet<string> DoNotRegisterMethodList = new HashSet<string>{ "OPTIONS" };

        public static void RegisterHit(this Controller controller, string displayName, int? idParameter = null)
        {
            if (!DoNotRegisterMethodList.Contains(controller.HttpContext.Request.Method))
            {
                Hit hit = new Hit();
                hit.ActionId = controller.ControllerContext.ActionDescriptor.Id;
                hit.ActionName = controller.ControllerContext.ActionDescriptor.ActionName;
                hit.ControllerName = controller.ControllerContext.ActionDescriptor.ControllerName;
                hit.IPHash = controller.HttpContext.Connection.RemoteIpAddress.GetHashCode();
                hit.IdParameter = idParameter;
                hit.HitTimestamp = DateTime.UtcNow;
                hit.ActionDisplayName = displayName;

                StatsService.Instance.AddHit(hit);
            }
        }

        public static Language ReadLanguageCookie(this Controller controller, SqlConnector sql)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            LanguageModel model = db.SelectLanguageModel();

            string twoLetterName;
            if (controller.Request.Cookies["language"] != null)
            {
                twoLetterName = controller.Request.Cookies["language"].ToString();
            }
            else
            {
                twoLetterName = model.DefaultLanguage.TwoLetterISOName;
            }

            var result = model.VerifyLanguage(twoLetterName);

            return result;
        }

        public static Language VerifyLanguage(this Controller controller, SqlConnector sql, int languageId)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            LanguageModel model = db.SelectLanguageModel();

            var result = model.VerifyLanguage(languageId);

            return result;
        }
        public static Language VerifyLanguage(this Controller controller, SqlConnector sql, string languageName)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            LanguageModel model = db.SelectLanguageModel();

            var result = model.VerifyLanguage(languageName);

            return result;
        }
    }
}
