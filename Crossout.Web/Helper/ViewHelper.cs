using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Models.General;
using Crossout.Web.Models.Items;

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
                title += $" - {item.Title}";
            }
            return title;
        }
    }
}
