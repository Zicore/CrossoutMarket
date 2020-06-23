using System;
using System.Collections.Generic;
using System.Linq;
using Crossout.AspWeb;
using Crossout.Web.Cache;
using Crossout.AspWeb.Models;
using Crossout.AspWeb.Models.Charts;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;
using Newtonsoft.Json;
using Crossout.Model.Items;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Services;
using Crossout.AspWeb.Models.Language;

namespace Crossout.AspWeb.Controllers
{
    public class DataController : Controller
    {
        private readonly RootPathHelper pathProvider;
        public DataController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        [Route("/data/search")]
        public IActionResult SearchData(string l)
        {
            Language lang = this.VerifyLanguage(sql, l);
            return RouteSearchData(lang.Id);
        }

        [Route("/data/item/all/{id}")]
        public IActionResult Chart(int id)
        {
            return RouteChartDataWithCache(id);
        }

        [Route("/data/item/all-full/{id}")]
        public IActionResult ChartFull(int id)
        {
            return RouteChartData(id, 360);
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private IActionResult RouteSearchData(int language)
        {
            sql.Open(WebSettings.Settings.CreateDescription());
            string sqlQuery = DataService.BuildSearchQuery(false, true, false, false, false, false, false, true, true, false);

            language = Math.Max(language, 1);
            var p = new Parameter { Identifier = "@language", Value = language };
            List<Parameter> parameters = new List<Parameter> { p };

            var ds = sql.SelectDataSet(sqlQuery, parameters);
            var model = new SearchDataContainer();
            foreach (var row in ds)
            {
                Item item = Item.Create(row);
                item.SetImageExists(pathProvider);
                // CrossoutDataService.Instance.AddData(item);
                model.Data.Add(item);
            }

            return Json(model);
        }

        private static readonly BasicCache<int, ChartDataModel> Cache = new BasicCache<int, ChartDataModel>();

        private IActionResult RouteChartDataWithCache(int id)
        {
            var model = Cache.Get(id, LoadModel, DateTime.Now, new TimeSpan(0, 0, 5, 0));
            return Json(model.Value);
        }

        private IActionResult RouteChartData(int id, int interval)
        {
            var model = LoadModel(id, interval);
            return Json(model);
        }

        // function for cache
        private ChartDataModel LoadModel(int id)
        {
            return LoadModel(id, 90);
        }

        private ChartDataModel LoadModel(int id, int interval)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            string name = "all";

            string query =
                "(SELECT marketrecent.id,marketrecent.sellprice,marketrecent.buyprice,marketrecent.selloffers,marketrecent.buyorders,marketrecent.datetime FROM marketrecent where marketrecent.itemnumber = @id and marketrecent.datetime BETWEEN DATE_SUB(NOW(), INTERVAL 7 DAY) AND NOW() ORDER BY marketrecent.Datetime desc LIMIT 5000) ORDER BY datetime ASC, id ASC";

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

            //string targetTimeEnd = (highResData.Count > 0) ? ConvertDateTimeToDBString(highResData.First().Timestamp) : ConvertDateTimeToDBString(DateTime.UtcNow);
            var targetTimeEnd = (highResData.Count > 0) ? highResData.First().Timestamp : DateTime.UtcNow;
            var additionalWhereClause = (highResData.Count > 0) ? "AND marketgrouped.datetime > DATE_SUB(@time, INTERVAL @interval DAY) " : "";
            query = $"(SELECT marketgrouped.id,marketgrouped.sellprice,marketgrouped.buyprice,marketgrouped.selloffers,marketgrouped.buyorders,marketgrouped.datetime FROM marketgrouped where marketgrouped.itemnumber = @id AND marketgrouped.datetime < @time {additionalWhereClause}ORDER BY marketgrouped.Datetime desc LIMIT 11000) ORDER BY id ASC;";
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

            return model;
        }

        private string ConvertDateTimeToDBString(DateTime dt)
        {
            return $"{dt.Year}-{dt.Month}-{dt.Day} {dt.Hour}:{dt.Minute}:{dt.Second}";
        }
    }

    public class SearchDataContainer
    {
        [JsonProperty("data")]
        public List<Item> Data = new List<Item>();
    }
}