using System;
using System.Collections.Generic;
using System.Linq;
using Crossout.Web.Models;
using Crossout.Web.Models.Charts;
using Nancy;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Modules.Data
{
    public class ChartDataModule : NancyModule
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);
        public ChartDataModule()
        {
            // would capture routes to /products/list sent as a GET request
            Get["/data/item/{name}/{id:int}"] = x =>
            {
                sql.Open(WebSettings.Settings.CreateDescription());

                string name = x.name;

                // Harcoded Limit of data points for now
                // we will aggregate data soon, we should never have so much data hopefully
                string query =
                    "(SELECT market.id,market.sellprice,market.buyprice,market.selloffers,market.buyorders,market.datetime FROM market where market.itemnumber = @id and market.datetime BETWEEN DATE_SUB(NOW(), INTERVAL 7 DAY) AND NOW() ORDER BY market.Datetime desc LIMIT 5000) ORDER BY datetime ASC, id ASC";
                              
                var p = new Parameter { Identifier = "@id", Value = x.id };
                var parmeter = new List<Parameter>();
                parmeter.Add(p);

                var ds = sql.SelectDataSet(query, parmeter);

                ChartDataModel model = new ChartDataModel();
                model.Name = name;
                if (ds != null && ds.Count > 0)
                {
                    foreach (var row in ds)
                    {
                        ChartItem item = ChartItem.CreateForChart(row);
                        model.Items.Add(item);
                    }
                }

                string targetTimeEnd = ConvertDateTimeToDBString(model.Items.First().Timestamp);
                query = "(SELECT marketgrouped.id,marketgrouped.sellprice,marketgrouped.buyprice,marketgrouped.selloffers,marketgrouped.buyorders,marketgrouped.datetime FROM marketgrouped where marketgrouped.itemnumber = @id AND marketgrouped.datetime < @time AND marketgrouped.datetime > DATE_SUB(@time, INTERVAL 90 DAY) ORDER BY marketgrouped.Datetime desc LIMIT 5000) ORDER BY id ASC;";
                p = new Parameter { Identifier = "@id", Value = x.id };
                var p2 = new Parameter { Identifier = "@time", Value = targetTimeEnd };
                parmeter = new List<Parameter>();
                parmeter.Add(p);
                parmeter.Add(p2);

                ds = sql.SelectDataSet(query, parmeter);

                if (ds != null && ds.Count > 0)
                {
                    for (int i = ds.Count - 1; i >= 0; i--)
                    {
                        ChartItem item = ChartItem.CreateForChart(ds[i]);
                        model.Items.Insert(0, item);
                    }
                }

                return Response.AsJson(model);
            };
        }

        private string ConvertDateTimeToDBString(DateTime dt)
        {
            return $"{dt.Year}-{dt.Month}-{dt.Day} {dt.Hour}:{dt.Minute}:{dt.Second}";
        }
    }
}
