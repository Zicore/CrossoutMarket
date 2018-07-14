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
                if (Name == "all")
                {
                    var data = new object[4][][];

                    for (int i = 0; i < 4; i++)
                    {
                        data[i] = new object[Items.Count][];
                        for(int j = 0; j < Items.Count; j++)
                        {
                            data[i][j] = new object[2];
                            data[i][j][0] = Items[j].UnixTimestamp;
                            if (i == 0)
                            {
                                data[i][j][1] = Items[j].FormatSellPrice;
                            }
                            else if (i == 1)
                            {
                                data[i][j][1] = Items[j].FormatBuyPrice;
                            }
                            else if (i == 2)
                            {
                                data[i][j][1] = Items[j].SellOffers;
                            }
                            else if (i == 3)
                            {
                                data[i][j][1] = Items[j].BuyOrders;
                            }

                        }
                    }

                    return data;
                }
                else
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
                        else if (Name == "buy")
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
