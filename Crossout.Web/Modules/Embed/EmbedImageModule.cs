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
using Crossout.Web.Models;
using Crossout.Web.Models.Charts;
using LineChart;

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

                string query = "SELECT market.id,market.sellprice,market.buyprice,market.selloffers,market.buyorders,market.datetime FROM market WHERE market.itemnumber = @id AND datetime > DATE_SUB(CURRENT_TIMESTAMP, INTERVAL @days DAY);";
                var p = new Parameter { Identifier = "@id", Value = id };
                var p2 = new Parameter { Identifier = "@days", Value = 3 };
                var parmeter = new List<Parameter>();
                parmeter.Add(p);
                parmeter.Add(p2);

                List<DataPoint> itemData = new List<DataPoint>();

                var ds = sql.SelectDataSet(query, parmeter);
                if (ds != null && ds.Count > 0)
                {
                    foreach (var row in ds)
                    {
                        itemData.Add(ChartItem.CreateForMinimalChart(row));
                    }
                }

                DataService db = new DataService(sql);

                var itemModel = db.SelectItem(id, false);

                EmbedImageCreator eic = new EmbedImageCreator(itemModel.Item, itemData);
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
