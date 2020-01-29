using Crossout.AspWeb.Models.Stats;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Helper
{
    public static class ControllerExtensions
    {
        public static void RegisterHit(this Controller controller, string displayName, int? idParameter = null)
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
            Console.WriteLine($"Registered Hit by {hit.IPHash} using Controller {hit.ControllerName} executing Action {hit.ActionName} with ID {hit.ActionId}. IdParameter is {hit.IdParameter}");
        }
    }
}
