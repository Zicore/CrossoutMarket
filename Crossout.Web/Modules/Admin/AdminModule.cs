using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Authentication.Forms;

namespace Crossout.Web.Modules.Admin
{
    public class AdminModule : NancyModule
    {
        public AdminModule()
        {
            Get["/admin"] = x =>
            {
                return View["admin"];
            };

            Get["/admin/logout"] = x =>
            {
                return this.LogoutAndRedirect("~/");
            };
        }
    }
}
