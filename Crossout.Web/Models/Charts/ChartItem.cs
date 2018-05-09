using System;
using LineChart;

namespace Crossout.Web.Models.Charts
{
    public class ChartItem
    {
        private DateTime _timestamp;
        public int Id { get; set; }
        
        public int SellOffers { get; set; }

        public int SellPrice { get; set; }

        public int BuyOrders { get; set; }

        public int BuyPrice { get; set; }
        
        public int Margin
        {
            get { return  (int)(SellPrice - BuyPrice - (SellPrice * 0.1d)); }
        }
        
        public decimal FormatMargin
        {
            get { return FormatPrice(Margin); }
        }
        
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                _timestamp = value;
                UnixTimestamp = UnixTicks(value);
            }
        }

        public double UnixTimestamp { get; set; }
        
        public decimal FormatBuyPrice
        {
            get
            {
                return FormatPrice(BuyPrice);
            }
        }
        
        public decimal FormatSellPrice
        {
            get
            {
                return FormatPrice(SellPrice);
            }
        }
        
        public decimal FormatPrice(int price)
        {
            return price / 100m;
        }
        
        public static ChartItem CreateForChart(object[] row)
        {
            int i = 0;
            ChartItem item = new ChartItem
            {
                Id = Convert.ToInt32(row[i++]),
                SellPrice = Convert.ToInt32(row[i++]),
                BuyPrice = Convert.ToInt32(row[i++]),
                SellOffers = Convert.ToInt32(row[i++]),
                BuyOrders = Convert.ToInt32(row[i++]),
                Timestamp = Convert.ToDateTime(row[i++]),
            };
            return item;
        }

        public static DataPoint CreateForMinimalChart(object[] row)
        {
            int i = 0;
            ChartItem item = new ChartItem
            {
                Id = Convert.ToInt32(row[i++]),
                SellPrice = Convert.ToInt32(row[i++]),
                BuyPrice = Convert.ToInt32(row[i++]),
                SellOffers = Convert.ToInt32(row[i++]),
                BuyOrders = Convert.ToInt32(row[i++]),
                Timestamp = Convert.ToDateTime(row[i++]),
            };

            var dataPoint = new DataPoint(UnixTicks(item.Timestamp), item.SellPrice);

            return dataPoint;
        }

        public static double UnixTicks(DateTime dt)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dt;
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return ts.TotalMilliseconds;
        }
    }
}
