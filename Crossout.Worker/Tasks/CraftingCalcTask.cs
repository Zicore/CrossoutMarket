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
        private enum WorkbenchItemId
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
        private enum Rarity
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

                SelectWorkbenchItems(sql);

                string collumns = "recipe.itemnumber,recipeitem.itemnumber,recipeitem.number,i1.sellprice,i1.buyprice,i1.amount,i2.raritynumber,i2.workbenchrarity";
                string query = $"SELECT {collumns} FROM recipe LEFT JOIN recipeitem ON recipeitem.recipenumber = recipe.id LEFT JOIN item i1 ON i1.id = recipeitem.itemnumber LEFT JOIN item i2 ON i2.id = recipe.itemnumber ORDER BY recipe.itemnumber";
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
                    int workbenchId = (int)GetWorkbenchItemIdByRarity((Rarity)raritynumber);
                    int minimumWorkbenchCost = workbenchPricesByID[workbenchId];

                    if (!craftingCostsByID.ContainsKey(id))
                    {
                        craftingCostsByID.Add(id, new CraftingSums() {
                            Id = id,
                            SellSum = DivideIntByIntAndRound(sellprice * multiplier, amount) + minimumWorkbenchCost,
                            BuySum = DivideIntByIntAndRound(buyprice * multiplier, amount) + minimumWorkbenchCost
                        });
                    }
                    else
                    {
                        craftingCostsByID[id].SellSum += DivideIntByIntAndRound(sellprice * multiplier, amount);
                        craftingCostsByID[id].BuySum += DivideIntByIntAndRound(buyprice * multiplier, amount);
                    }
                }

                foreach(var item in craftingCostsByID)
                {
                    List<Parameter> parameters = new List<Parameter>();
                    parameters.Add(new Parameter { Identifier = "@id", Value = item.Key });
                    parameters.Add(new Parameter { Identifier = "@sellsum", Value = item.Value.SellSum });
                    parameters.Add(new Parameter { Identifier = "@buysum", Value = item.Value.BuySum });
                    try
                    {
                        var result = sql.ExecuteSQL("UPDATE item SET item.craftingsellsum = @sellsum, item.craftingbuysum = @buysum WHERE item.id = @id", parameters);
                    }
                    catch
                    {
                        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} failed.");
                        isRunning = false;
                        return;
                    }
                }

                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} finished!");
                isRunning = false;
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} already running, skipping.");
            }
        }

        private int DivideIntByIntAndRound(int dividend, int divisor)
        {
            return (int)Math.Round(decimal.Divide(dividend, divisor));
        }

        private static WorkbenchItemId GetWorkbenchItemIdByRarity(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common_1:
                    return WorkbenchItemId.Common_445;
                case Rarity.Rare_2:
                    return WorkbenchItemId.Rare_446;
                case Rarity.Epic_3:
                    return WorkbenchItemId.Epic_447;
                case Rarity.Legendary_4:
                    return WorkbenchItemId.Legendary_448;
                case Rarity.Relic_5:
                    return WorkbenchItemId.Relic_449;
                case Rarity.Skins_6:
                    return WorkbenchItemId.Skins_466;
                default: return WorkbenchItemId.Common_445;
            }
        }

        private void SelectWorkbenchItems(SqlConnector sql)
        {
            StringBuilder sb = new StringBuilder();
            string query = "SELECT item.id,item.sellprice FROM crossout.item WHERE ";
            sb.Append(query);
            foreach(int item in Enum.GetValues(typeof(WorkbenchItemId)))
            {
                sb.Append($"item.id={item} OR ");
            }
            sb.Append("null");
            try
            {
                var ds = sql.SelectDataSet(sb.ToString());
                workbenchPricesByID.Clear();
                foreach (var row in ds)
                {
                    workbenchPricesByID.Add((int)row[0], (int)row[1]);
                }
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} failed.");
                isRunning = false;
                return;
            }
        }
    }
}
