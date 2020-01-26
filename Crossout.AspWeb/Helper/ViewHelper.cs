using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Models.Items;
using Crossout.AspWeb.Models.View;

namespace Crossout.AspWeb.Helper
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
