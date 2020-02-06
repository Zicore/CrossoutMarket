using Crossout.Toolkit.Helper;
using Crossout.Toolkit.Models.LocNameImporter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Toolkit.Tools
{
    public static class LocNameImporter
    {
        private static List<LocItem> locItems = new List<LocItem>();
        private static List<DBItem> existingDBItems = new List<DBItem>();
        private static List<DBLocalization> existingDBLocalizations = new List<DBLocalization>();

        private static SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        public static void Execute(string filePath)
        {
            ImportLocItemsFromFile(filePath);

            sql.Open(ToolkitSettings.Settings.CreateDescription());
            ImportDBItems();
            ImportDBLocalizations();

            foreach (var item in existingDBItems)
            {
                var matchingLoc = locItems.Find(x => x.Name == item.Def);

                if (matchingLoc != null)
                {
                    var existingLoc = existingDBLocalizations.FindAll(x => x.ItemId == item.Id);

                    if (existingLoc.Count > 0)
                    {
                        foreach (var loc in existingLoc)
                        {
                            switch (loc.LanguageId)
                            {
                                case 1:
                                    if (loc.LocName != matchingLoc.EnName)
                                    {
                                        var updatedLoc = new DBLocalization
                                        {
                                            Id = loc.Id,
                                            LanguageId = loc.LanguageId,
                                            ItemId = loc.ItemId,
                                            LocName = matchingLoc.EnName
                                        };
                                        UpdateExistingLoc(updatedLoc);
                                    }
                                    break;
                                case 2:
                                    if (loc.LocName != matchingLoc.RuName)
                                    {
                                        var updatedLoc = new DBLocalization
                                        {
                                            Id = loc.Id,
                                            LanguageId = loc.LanguageId,
                                            ItemId = loc.ItemId,
                                            LocName = matchingLoc.RuName
                                        };
                                        UpdateExistingLoc(updatedLoc);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private static void ImportLocItemsFromFile(string filePath)
        {
            string jsonString = FileReader.ReadFile(filePath);
            var locNameFile = JsonConvert.DeserializeObject<LocNameFile>(jsonString);
            locItems = locNameFile.Res;
            Console.WriteLine($"Read {locItems.Count} Items from Loc Name File");
        }

        private static void ImportDBItems()
        {
            string query = "SELECT item.id, item.name, item.externalKey FROM item";

            var ds = sql.SelectDataSet(query);

            foreach (var row in ds)
            {
                int i = 0;
                var dbItem = new DBItem
                {
                    Id = Convert.ToInt32(row[i++]),
                    Def = Convert.ToString(row[i++]),
                    Name = Convert.ToString(row[i++])
                };
                existingDBItems.Add(dbItem);
            }

            Console.WriteLine($"Read {existingDBItems.Count} Existing Items from DB");

        }

        private static void ImportDBLocalizations()
        {
            string query = "SELECT itemlocalization.id, itemlocalization.languagenumber, itemlocalization.itemnumber, itemlocalization.localizedname FROM itemlocalization";

            var ds = sql.SelectDataSet(query);

            foreach (var row in ds)
            {
                int i = 0;
                var dbLocalization = new DBLocalization
                {
                    Id = Convert.ToInt32(row[i++]),
                    LanguageId = Convert.ToInt32(row[i++]),
                    ItemId = Convert.ToInt32(row[i++]),
                    LocName = Convert.ToString(row[i++])
                };
                existingDBLocalizations.Add(dbLocalization);
            }

            Console.WriteLine($"Read {existingDBItems.Count} Existing Localizations from DB");

        }

        private static void UpdateExistingLoc(DBLocalization loc)
        {

        }
    }
}
