using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web;

namespace Crossout.AspWeb.Helper
{
    public static class FileExtensions
    {
        public static void SetImageExists(this Item item, RootPathHelper rootPathHelper)
        {
            item.ImageExists = rootPathHelper.ImageExists(item.Image);
            item.HighResImageExists = rootPathHelper.HighResImageExists(item.Image);
        }
    }
}
