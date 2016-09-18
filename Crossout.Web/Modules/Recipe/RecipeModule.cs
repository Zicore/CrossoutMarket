using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crossout.Web.Models;
using Nancy;

namespace Crossout.Web.Modules.Recipe
{
    public class RecipeModule : NancyModule
    {
        public RecipeModule()
        {
            Get["/recipe/"] = x =>
            {
                RecipeModel model = new RecipeModel();

                return View["recipe"];
            };
        }
    }
}
