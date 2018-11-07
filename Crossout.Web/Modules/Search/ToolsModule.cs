using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Model.Recipes;
using Crossout.Web.Models.General;
using Crossout.Web.Services;
using Crossout.Model.Formatter;
using Nancy;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Modules.Search
{
    public class ToolsModule : NancyModule
    {
        public ToolsModule()
        {
            Get["/tools"] = x =>
            {
                ToolsModel model = new ToolsModel();
                return View["tools", model];
            };
        }
    }
}
