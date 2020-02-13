using Crossout.Toolkit.Helper;
using Crossout.Toolkit.Models.APICook;
using Crossout.Toolkit.Models.LocNameImporter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Crossout.Toolkit.Tools
{
    public static class Reparser
    {
        public static void Execute()
        {
            string jsonString = FileReader.ReadFile("Data\\APICook\\manualgathering.json");
            var manualGatheringInput = JsonConvert.DeserializeObject<List<ItemDef>>(jsonString);

            var output = new List<LocItem>();
            foreach (var input in manualGatheringInput)
            {
                output.Add(new LocItem { 
                    Name = input.Name,
                    EnName = input.LocName,
                    Rarity = "NONE",
                    Subtype = input.SubType,
                    RuName = "TBD"
                });
            }

            var outputJson = JsonConvert.SerializeObject(output);
            using (StreamWriter sw = new StreamWriter("ReparserOutput.json"))
            {
                sw.Write(outputJson);
            }

            Console.WriteLine($"Read {manualGatheringInput.Count} Items from Loc Name File");
        }
    }
}
