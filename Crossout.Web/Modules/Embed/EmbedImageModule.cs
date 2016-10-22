using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Crossout.Model.Recipes;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.Web.Services;
using System.IO;
using Crossout.Images;
using System.Drawing;
using Crossout.Web.Responses;
using System.Drawing.Imaging;

namespace Crossout.Web.Modules.Data
{
    public class ImageModule : NancyModule
    {
        public ImageModule()
        {
            Get["embed/image/{id:int}.{extension}"] = x =>
            {
                var id = (int)x.id;
                return RouteImage(id);
            };

            Get["embed/image/{id:int}"] = x =>
            {
                var id = (int)x.id;
                return RouteImage(id);
            };
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private dynamic RouteImage(int id)
        {
            try
            {
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemModel = db.SelectItem(id);

                EmbedImageCreator eic = new EmbedImageCreator(itemModel.Item);
                var imageArray = (byte[])new ImageConverter().ConvertTo(eic.CreateEmbedImage(), typeof(byte[]));

                return Response.FromByteArray(imageArray, "image / png");
            }
            catch
            {
                return Response.AsRedirect("/");
            }

            
        }
    }
}
