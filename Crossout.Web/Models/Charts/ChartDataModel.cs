using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crossout.Web.Models.Charts
{
    public class ChartDataModel
    {
        [JsonIgnore]
        public List<ChartItem> Items  = new List<ChartItem>();

        public string Name { get; set; }
        public int Id { get; set; }

        public object[][] Data
        {
            get
            {
                var data = new object[Items.Count][];

                for (int i = 0; i < Items.Count; i++)
                {
                    object[] row = new object[2];
                    row[0] = Items[i].UnixTimestamp;
                    if (Name == "sell")
                    {
                        row[1] = Items[i].FormatSellPrice;
                    }
                    else if(Name == "buy")
                    {
                        row[1] = Items[i].FormatBuyPrice;
                    }
                    else if (Name == "selloffers")
                    {
                        row[1] = Items[i].SellOffers;
                    }
                    else if (Name == "buyorders")
                    {
                        row[1] = Items[i].BuyOrders;
                    }
                    data[i] = row;
                }

                return data;
            }
        }

        //public object[][] DataColumns
        //{
        //    get
        //    {
        //        var data = new object[Items.Count][];

        //        for (int i = 0; i < Items.Count; i++)
        //        {
        //            object[] row = new object[2];
        //            row[0] = Items[i].UnixTimestamp;
        //            if (Name == "sell")
        //            {
        //                row[1] = Items[i].BuyOrders;
        //            }
        //            else if (Name == "buy")
        //            {
        //                row[1] = Items[i].SellOffers;
        //            }

        //            data[i] = row;
        //        }

        //        return data;
        //    }
        //}
    }
}
