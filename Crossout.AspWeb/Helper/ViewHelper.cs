using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Models.General;
using Crossout.Web.Models.Items;
using Crossout.Web.Models.View;

namespace Crossout.Web.Helper
{
    public class ViewHelper
    {
        public static string GenerateTitle(IViewTitle item)
        {
            System.Diagnostics.StackTrace s = new StackTrace();
            
            string title = WebSettings.Title;
            if (!string.IsNullOrEmpty(item?.Title))
            {
                title = $"{item.Title} | {WebSettings.Title}";
            }
            return title;
        }

        public static string GenerateTitlePosition(IViewTitle item)
        {
            System.Diagnostics.StackTrace s = new StackTrace();

            string title = "";
            if (!string.IsNullOrEmpty(item?.Title))
            {
                title += $"{item.Title}";
            }
            return title;
        }

        public static string GetHostWithPort()
        {
            //if (request.Url.Port == 80)
            //{
            //    return request.Url.HostName;
            //}
            //return $"{request.Url.HostName}:{request.Url.Port}";
            return "";
        }
    }
}
