using Crossout.Toolkit.Helper;
using Crossout.Toolkit.Models.APICook;
using Crossout.Toolkit.Models.LocNameImporter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Crossout.Toolkit.Tools
{
    public static class APICook
    {
        private static List<LocItem> locItems = new List<LocItem>();

        public static void Execute(string filePath)
        {
            string jsonString = FileReader.ReadFile(filePath);
            var locNameFile = JsonConvert.DeserializeObject<LocNameFile>(jsonString);
            locItems = locNameFile.Res;

            List<ItemDef> itemDefs = new List<ItemDef>();

            foreach (var loc in locItems)
            {
                itemDefs.Add(new ItemDef
                {
                    Name = loc.Name,
                    LocName = loc.EnName,
                    Rarity = 0,
                    SubType = "NONE"
                });
            }
            Console.WriteLine($"Read {itemDefs.Count} Items from Loc Name File");

            var manualGatheringString = FileReader.ReadFile("Data\\APICook\\manualgathering.json");
            var manualGatheringObject = JsonConvert.DeserializeObject<JArray>(manualGatheringString);
            foreach (var item in manualGatheringObject)
            {
                itemDefs.Add(JsonConvert.DeserializeObject<ItemDef>(item.ToString()));
            }

            HttpClient client = new HttpClient();
            var apiUrl = $"https://ah-snapshots.crossout.net/ah-snapshoter/snapshots?from=" + DateTime.UtcNow.AddMinutes(-5).Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds.ToString("0");

            Console.WriteLine($"Contacting API: " + apiUrl);
            HttpResponseMessage response = client.GetAsync(apiUrl).Result;

            Console.WriteLine($"API Responded");

            var marketJsonString = response.Content.ReadAsStringAsync().Result;
            var marketJsonObject = JsonConvert.DeserializeObject<JObject>(marketJsonString);

            List<ItemDef> result = new List<ItemDef>();

            foreach (var jsonItem in marketJsonObject["res"].First.First)
            {
                var item = JsonConvert.DeserializeObject<MarketItem>(jsonItem.ToString());
                var matchingLoc = itemDefs.Find(x => x.Name == item.def);

                if (matchingLoc == null)
                {
                    Console.WriteLine($"Couldn't find locname for {item.def}");
                }
                else
                {
                    result.Add(matchingLoc);
                }
            }
            var resultSerialized = JsonConvert.SerializeObject(result);
            using (StreamWriter sw = new StreamWriter("APICookOutput.json"))
            {
                sw.Write(resultSerialized);
            }

            Console.WriteLine($"Finished writing {result.Count} Items to APICookOutput.json");

        }
    }
}
