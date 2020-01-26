using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.Model.Items;
using Crossout.Web;
using Crossout.AspWeb.Models.Filter;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Models.Pagination;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;
using System.ComponentModel;
using Crossout.AspWeb.Models.View;

namespace Crossout.AspWeb.Controllers
{
    public class TrendsController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public TrendsController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("trends")]
        public IActionResult Trends()
        {
            return Redirect("/");
            //return RouteTrends();
        }

        private IActionResult RouteTrends()
        {
            DataService db = new DataService(sql);

            sql.Open(WebSettings.Settings.CreateDescription());

            var parmeter = new List<Parameter>();

            var timeA = new DateTime(2020, 1, 17, 12, 0, 0);
            var timeB = new DateTime(2020, 1, 17, 20, 25, 3);

            string sqlQuery = DataService.BuildTrendsQuery(timeA);
            var dsA = sql.SelectDataSet(sqlQuery);
            sqlQuery = DataService.BuildTrendsQuery(timeB);
            var dsB = sql.SelectDataSet(sqlQuery);
            var trends = new List<TrendItem>();
            for (int i = 0; i < dsA.Count; i++)
            {
                var rowA = dsA[i];
                var rowB = dsB[i];
                var trendItem = new TrendItem();
                trendItem.Id = Convert.ToInt32(rowA[0]);
                trendItem.SellA = Convert.ToInt32(rowA[1]);
                trendItem.SellB = Convert.ToInt32(rowB[1]);
                trendItem.BuyA = Convert.ToInt32(rowA[2]);
                trendItem.BuyB = Convert.ToInt32(rowB[2]);
                trends.Add(trendItem);
            }

            var trendModel = new TrendModel();
            trendModel.TimeA = timeA;
            trendModel.TimeB = timeB;
            trendModel.Trends = trends;

            return View("trends", trendModel);
        }
    }

    public class TrendItem
    {
        public int Id { get; set; }
        public int SellA { get; set; }
        public int SellB { get; set; }
        public int BuyA { get; set; }
        public int BuyB { get; set; }
        public float SellTrend
        {
            get
            {
                if (SellA != 0 && SellB != 0)
                {
                    return ((float)SellB / (float)SellA) * 100f - 100f;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    public class TrendModel : IViewTitle
    {
        public List<TrendItem> Trends = new List<TrendItem>();
        public DateTime TimeA;
        public DateTime TimeB;

        public string Title => "Trends";
    }
}