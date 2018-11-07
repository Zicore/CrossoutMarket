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
            // "all" means all data types like sellprices, buyprices, selloffers and sellorders
            Get["/data/item/all/{id:int}"] = x =>
            {
                int id = x.id;
                return RouteChartData(id);
            };

            // loads more grouped data than "all"
            Get["/data/item/full/{id:int}"] = x =>
            {
                int id = x.id;
                return RouteChartData(id, 360);
            };
        }

        private dynamic RouteChartData(int id, int interval = 90)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            string name = "all";

            // Harcoded Limit of data points for now
            // we will aggregate data soon, we should never have so much data hopefully
            string query =
                "(SELECT market.id,market.sellprice,market.buyprice,market.selloffers,market.buyorders,market.datetime FROM market where market.itemnumber = @id and market.datetime BETWEEN DATE_SUB(NOW(), INTERVAL 7 DAY) AND NOW() ORDER BY market.Datetime desc LIMIT 5000) ORDER BY datetime ASC, id ASC";

            var p = new Parameter { Identifier = "@id", Value = id };
            var parmeter = new List<Parameter>();
            parmeter.Add(p);

            var ds = sql.SelectDataSet(query, parmeter);

            ChartDataModel model = new ChartDataModel();
            model.Name = name;
            List<ChartItem> highResData = new List<ChartItem>();
            if (ds != null && ds.Count > 0)
            {
                foreach (var row in ds)
                {
                    ChartItem item = ChartItem.CreateForChart(row);
                    highResData.Add(item);
                }
            }

            string targetTimeEnd = ConvertDateTimeToDBString(highResData.First().Timestamp);
            query = "(SELECT marketgrouped.id,marketgrouped.sellprice,marketgrouped.buyprice,marketgrouped.selloffers,marketgrouped.buyorders,marketgrouped.datetime FROM marketgrouped where marketgrouped.itemnumber = @id AND marketgrouped.datetime < @time AND marketgrouped.datetime > DATE_SUB(@time, INTERVAL @interval DAY) ORDER BY marketgrouped.Datetime desc LIMIT 11000) ORDER BY id ASC;";
            p = new Parameter { Identifier = "@id", Value = id };
            var p2 = new Parameter { Identifier = "@time", Value = targetTimeEnd };
            var p3 = new Parameter { Identifier = "@interval", Value = interval };
            parmeter = new List<Parameter>();
            parmeter.Add(p);
            parmeter.Add(p2);
            parmeter.Add(p3);

            ds = sql.SelectDataSet(query, parmeter);
            List<ChartItem> groupedData = new List<ChartItem>();
            if (ds != null && ds.Count > 0)
            {
                foreach (var row in ds)
                {
                    ChartItem item = ChartItem.CreateForChart(row);
                    groupedData.Add(item);
                }
            }

            model.Items.AddRange(groupedData);
            model.Items.AddRange(highResData);

            return Response.AsJson(model);
        }

        private string ConvertDateTimeToDBString(DateTime dt)
        {
            return $"{dt.Year}-{dt.Month}-{dt.Day} {dt.Hour}:{dt.Minute}:{dt.Second}";
        }
    }
}
