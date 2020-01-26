using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.WorkerCore.Models.SteamAPI;
using System.Threading;
using Newtonsoft.Json;
using Crossout.WorkerCore.Models.MarketGrouper;

namespace Crossout.WorkerCore.Tasks
{
    public class MarketGrouperTask : BaseTask
    {
        public MarketGrouperTask(string key) : base(key) { }

        private static List<MarketItem> groupedMarketItems = new List<MarketItem>();

        private static bool isRunning = false;

        public override void Workload(SqlConnector sql)
        {
            if (!isRunning)
            {
                isRunning = true;
                groupedMarketItems.Clear();
                string query = "SELECT marketgrouped.id,marketgrouped.datetime as datetime FROM marketgrouped ORDER BY marketgrouped.datetime DESC LIMIT 1";
                List<object[]> dataset = new List<object[]>();
                try
                {
                    dataset = sql.SelectDataSet(query);
                }
                catch
                {
                    Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} failed.");
                    isRunning = false;
                    return;
                }
                DateTime latestGroupedData;
                if (dataset.Count != 0)
                {
                    latestGroupedData = Convert.ToDateTime(dataset.First()[1]);
                    DateTime targetTimeStart = latestGroupedData.AddHours(1);
                    targetTimeStart = TruncateMinutes(targetTimeStart);
                    DateTime targetTimeEnd = targetTimeStart.AddDays(1);

                    query = $"SELECT market.id,market.datetime as datetime FROM market ORDER BY market.datetime DESC LIMIT 1";
                    try
                    {
                        dataset = sql.SelectDataSet(query);
                    }
                    catch
                    {
                        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} failed.");
                        isRunning = false;
                        return;
                    }

                    if (targetTimeEnd < Convert.ToDateTime(dataset.First()[1]).AddHours(-1))
                    {
                        bool isSearchingData = true;
                        while (isSearchingData)
                        {
                            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} is grouping data from {targetTimeStart.ToString()} to {targetTimeEnd.ToString()}");

                            query = $"SELECT market.id,market.itemnumber,market.sellprice,market.buyprice,market.selloffers,market.buyorders,market.datetime as datetime FROM market WHERE market.datetime >= '{ConvertDateTimeToDBString(targetTimeStart)}' AND market.datetime < '{ConvertDateTimeToDBString(targetTimeEnd)}' ORDER BY market.itemnumber ASC";
                            try
                            {
                                dataset = sql.SelectDataSet(query);
                            }
                            catch
                            {
                                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} failed.");
                                isRunning = false;
                                return;
                            }

                            if (dataset.Count != 0)
                            {
                                isSearchingData = false;
                                Dictionary<DateTime, Dictionary<int, List<MarketItem>>> groupedList = new Dictionary<DateTime, Dictionary<int, List<MarketItem>>>();

                                foreach (var row in dataset)
                                {
                                    MarketItem item = new MarketItem()
                                    {
                                        Id = (int)row[0],
                                        ItemNumber = (int)row[1],
                                        SellPrice = (int)row[2],
                                        BuyPrice = (int)row[3],
                                        SellOffers = (int)row[4],
                                        BuyOrders = (int)row[5],
                                        DateTime = (DateTime)row[6]
                                    };
                                    var truncatedDateTime = TruncateMinutes(item.DateTime);
                                    if (!groupedList.ContainsKey(truncatedDateTime))
                                    {
                                        groupedList.Add(truncatedDateTime, new Dictionary<int, List<MarketItem>>());
                                    }

                                    if (!groupedList[truncatedDateTime].ContainsKey(item.ItemNumber))
                                    {
                                        groupedList[truncatedDateTime].Add(item.ItemNumber, new List<MarketItem>() { item });
                                    }
                                    else
                                    {
                                        groupedList[truncatedDateTime][item.ItemNumber].Add(item);
                                    }
                                }

                                foreach (var timegroup in groupedList)
                                {
                                    foreach (var itemgroup in timegroup.Value)
                                    {
                                        double sellSum = 0;
                                        double buySum = 0;
                                        double offerSum = 0;
                                        double orderSum = 0;
                                        double sellAverage = 0;
                                        double buyAverage = 0;
                                        double offerAverage = 0;
                                        double orderAverage = 0;

                                        foreach (var item in itemgroup.Value)
                                        {
                                            sellSum += item.SellPrice;
                                            buySum += item.BuyPrice;
                                            offerSum += item.SellOffers;
                                            orderSum += item.BuyOrders;
                                        }

                                        sellAverage = sellSum / itemgroup.Value.Count;
                                        buyAverage = buySum / itemgroup.Value.Count;
                                        offerAverage = offerSum / itemgroup.Value.Count;
                                        orderAverage = orderSum / itemgroup.Value.Count;

                                        var resultItem = itemgroup.Value.First();
                                        resultItem.SellPrice = (int)Math.Round(sellAverage);
                                        resultItem.BuyPrice = (int)Math.Round(buyAverage);
                                        resultItem.SellOffers = (int)Math.Round(offerAverage);
                                        resultItem.BuyOrders = (int)Math.Round(orderAverage);
                                        resultItem.DateTime = TruncateMinutes(resultItem.DateTime);

                                        groupedMarketItems.Add(resultItem);
                                    }
                                }

                                groupedMarketItems = groupedMarketItems.OrderBy(x => x.DateTime).ToList();
                                InsertGroupedItems(groupedMarketItems, sql);


                            }
                            else
                            {
                                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} couldn't find data to group at given the time");
                                targetTimeStart = targetTimeStart.AddDays(1);
                                targetTimeEnd = targetTimeStart.AddDays(1);
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} couldn't find any work to do");
                    }


                }
                else
                {
                    Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} couldn't find any entries in marketgrouped table");
                }

                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} finished!");
                isRunning = false;
            }
        }

        private string ConvertDateTimeToDBString(DateTime dt)
        {
            return $"{dt.Year}-{dt.Month}-{dt.Day} {dt.Hour}:{dt.Minute}:{dt.Second}";
        }

        private DateTime TruncateMinutes(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0); ;
        }

        private void InsertGroupedItems(List<MarketItem> items, SqlConnector sql)
        {
            string query = "INSERT INTO marketgrouped (itemnumber,sellprice,buyprice,selloffers,buyorders,datetime) VALUES";
            StringBuilder sb = new StringBuilder(query);
            foreach (var item in items)
            {
                sb.Append("('");
                sb.Append(item.ItemNumber);
                sb.Append("','");
                sb.Append(item.SellPrice);
                sb.Append("','");
                sb.Append(item.BuyPrice);
                sb.Append("','");
                sb.Append(item.SellOffers);
                sb.Append("','");
                sb.Append(item.BuyOrders);
                sb.Append("','");
                sb.Append(ConvertDateTimeToDBString(item.DateTime));
                sb.Append("')");
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(";");
            query = sb.ToString();
            QueryResult result = new QueryResult();
            try
            {
                result = sql.ExecuteSQL(query);
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} failed.");
                isRunning = false;
                return;
            }
            Console.WriteLine($"Query result: Error={result.HasError}, Rows inserted={result.RowCount}, LastID={result.LastInsertedId}");
        }
    }
}
