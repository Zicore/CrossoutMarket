using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.Worker.Models.SteamAPI;
using System.Threading;
using Newtonsoft.Json;
using Crossout.Worker.Models.CraftingCalc;

namespace Crossout.Worker.Tasks
{
    public class CraftingCalcTask : BaseTask
    {
        public CraftingCalcTask(string key) : base(key) { }

        private static Dictionary<int, CraftingSums> craftingCostsByID = new Dictionary<int, CraftingSums>();
        private static Dictionary<int, int> workbenchPricesByID = new Dictionary<int, int>();
        private static bool isRunning = false;
        private enum workbenchItemId
        {
            //445	Common Minimum Bench Cost
            //446	Rare Minimum Bench Cost
            //447	Epic Minimum Bench Cost
            //448	Legendary Minimum Bench Cost
            //449	Relic Minimum Bench Cost

            Common_445 = 445,
            Rare_446 = 446,
            Epic_447 = 447,
            Legendary_448 = 448,
            Relic_449 = 449,
            Skins_466 = 466,
        }
        private enum rarity
        {
            Common_1 = 1,
            Rare_2 = 2,
            Epic_3 = 3,
            Legendary_4 = 4,
            Relic_5 = 5,
            Skins_6 = 6
        }

        public override void Workload(SqlConnector sql)
        {
            if (!isRunning)
            {
                isRunning = true;

                selectWorkbenchItems(sql);

                string collumns = "recipe.itemnumber,recipeitem.itemnumber,recipeitem.number,i1.sellprice,i1.buyprice,i1.amount,i2.raritynumber,i2.workbenchrarity";
                string query = $"SELECT {collumns} FROM recipe LEFT JOIN recipeitem ON recipeitem.recipenumber = recipe.id LEFT JOIN item i1 ON i1.id = recipeitem.itemnumber LEFT JOIN item i2 ON i2.id = recipe.itemnumber ORDER BY recipe.itemnumber";
                var dataset = sql.SelectDataSet(query);

                craftingCostsByID.Clear();

                foreach (var row in dataset)
                {
                    int id = (int)row[0];
                    int ingredientid = (int)row[1];
                    int multiplier = (int)row[2];
                    int sellprice = (int)row[3];
                    int buyprice = (int)row[4];
                    int amount = (int)row[5];
                    int raritynumber = (int)row[6];
                    int workbenchrarity = (int)row[7];
                    if (workbenchrarity != 0)
                    {
                        raritynumber = workbenchrarity;
                    }
                    int workbenchId = (int)getWorkbenchItemIdByRarity((rarity)raritynumber);
                    int minimumWorkbenchCost = workbenchPricesByID[workbenchId];

                    if (!craftingCostsByID.ContainsKey(id))
                    {
                        craftingCostsByID.Add(id, new CraftingSums() {
                            Id = id,
                            SellSum = divideIntByIntAndRound(sellprice * multiplier, amount) + minimumWorkbenchCost,
                            BuySum = divideIntByIntAndRound(buyprice * multiplier, amount) + minimumWorkbenchCost
                        });
                    }
                    else
                    {
                        craftingCostsByID[id].SellSum += divideIntByIntAndRound(sellprice * multiplier, amount);
                        craftingCostsByID[id].BuySum += divideIntByIntAndRound(buyprice * multiplier, amount);
                    }
                }

                foreach(var item in craftingCostsByID)
                {
                    List<Parameter> parameters = new List<Parameter>();
                    parameters.Add(new Parameter { Identifier = "@id", Value = item.Key });
                    parameters.Add(new Parameter { Identifier = "@sellsum", Value = item.Value.SellSum });
                    parameters.Add(new Parameter { Identifier = "@buysum", Value = item.Value.BuySum });
                    var result = sql.ExecuteSQL("UPDATE item SET item.craftingsellsum = @sellsum, item.craftingbuysum = @buysum WHERE item.id = @id", parameters);
                }

                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} finished!");
                isRunning = false;
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} already running, skipping.");
            }
        }

        private int divideIntByIntAndRound(int dividend, int divisor)
        {
            return (int)Math.Round(decimal.Divide(dividend, divisor));
        }

        private workbenchItemId getWorkbenchItemIdByRarity(rarity rarity)
        {
            switch (rarity)
            {
                case rarity.Common_1:
                    return workbenchItemId.Common_445;
                case rarity.Rare_2:
                    return workbenchItemId.Rare_446;
                case rarity.Epic_3:
                    return workbenchItemId.Epic_447;
                case rarity.Legendary_4:
                    return workbenchItemId.Legendary_448;
                case rarity.Relic_5:
                    return workbenchItemId.Relic_449;
                case rarity.Skins_6:
                    return workbenchItemId.Skins_466;
                default: return workbenchItemId.Common_445;
            }
        }

        private void selectWorkbenchItems(SqlConnector sql)
        {
            StringBuilder sb = new StringBuilder();
            string query = "SELECT item.id,item.sellprice FROM crossout.item WHERE ";
            sb.Append(query);
            foreach(int item in Enum.GetValues(typeof(workbenchItemId)))
            {
                sb.Append($"item.id={item} OR ");
            }
            sb.Append("null");
            var ds = sql.SelectDataSet(sb.ToString());
            workbenchPricesByID.Clear();
            foreach (var row in ds)
            {
                workbenchPricesByID.Add((int)row[0], (int)row[1]);
            }
        }
    }
}
