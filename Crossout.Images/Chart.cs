using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LineChart
{
    public class Chart
    {
        //SqlConnector sql = new SqlConnector(ConnectionType.MySql);
        public List<Series> SeriesList { get; set; } = new List<Series>();

        public RectangleF Bounds { get; set; }

        public void AddSeries(List<DataPoint> data)
        {
            //sql.Open(LineChartSettings.Settings.CreateDescription());

            //string query = "SELECT market.id,market.sellprice,market.buyprice,market.selloffers,market.buyorders,market.datetime FROM market WHERE market.itemnumber = @id AND datetime > DATE_SUB(CURRENT_TIMESTAMP, INTERVAL @days DAY);";
            //var p = new Parameter { Identifier = "@id", Value = id };
            //var p2 = new Parameter { Identifier = "@days", Value = 7 };
            //var parmeter = new List<Parameter>();
            //parmeter.Add(p);
            //parmeter.Add(p2);

            //List<DataPoint> datapoints = new List<DataPoint>();

            //var ds = sql.SelectDataSet(query, parmeter);
            //if (ds != null && ds.Count > 0)
            //{
            //    foreach (var row in ds)
            //    {
            //        datapoints.Add(ChartItem.CreateForMinimalChart(row));
            //    }
            //}

            SeriesList.Add(new Series(){ Items = data });
        }

        public void Draw(Graphics g)
        {
            foreach (var series in SeriesList)
            {
                series.Draw(g, this);
            }
        }
    }
}
