using Crossout.AspWeb.Models.Stats;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
    }
}
