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
        private static List<LocItem> manualLocItems = new List<LocItem>();
        private static List<DBItem> existingDBItems = new List<DBItem>();
        private static List<DBLocalization> existingDBLocalizations = new List<DBLocalization>();
        private static List<DBLocalization> locItemToUpdate = new List<DBLocalization>();
        private static List<DBLocalization> locItemToInsert = new List<DBLocalization>();
        private static List<Language> languages = new List<Language>();

        private static SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        public static void Execute(string filePath)
        {
            ImportLocItemsFromFile(filePath);
            ImportManualGatheringFile();

            sql.Open(ToolkitSettings.Settings.CreateDescription());
            ImportDBItems();
            ImportDBLocalizations();
            ImportDBLanguages();

            foreach (var item in existingDBItems)
            {
                var matchingLoc = locItems.Find(x => x.Name == item.Def);

                if (matchingLoc == null)
                {
                    var manualLoc = manualLocItems.Find(x => x.Name == item.Def);
                    if (manualLoc != null)
                    {
                        matchingLoc = manualLoc;
                    }
                }

                if (matchingLoc != null)
                {
                    foreach (var lang in languages)
                    {
                        var existingLoc = existingDBLocalizations.Find(x => x.ItemId == item.Id && x.LanguageId == lang.Id);

                        if (existingLoc != null)
                        {
                            switch (existingLoc.LanguageId)
                            {
                                case 1:
                                    if (existingLoc.LocName != matchingLoc.EnName)
                                    {
                                        var updatedLoc = new DBLocalization
                                        {
                                            Id = existingLoc.Id,
                                            LanguageId = existingLoc.LanguageId,
                                            ItemId = existingLoc.ItemId,
                                            LocName = matchingLoc.EnName
                                        };
                                        locItemToUpdate.Add(updatedLoc);
                                    }
                                    break;
                                case 2:
                                    if (existingLoc.LocName != matchingLoc.RuName)
                                    {
                                        var updatedLoc = new DBLocalization
                                        {
                                            Id = existingLoc.Id,
                                            LanguageId = existingLoc.LanguageId,
                                            ItemId = existingLoc.ItemId,
                                            LocName = matchingLoc.RuName
                                        };
                                        locItemToUpdate.Add(updatedLoc);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            var newLoc = new DBLocalization()
                            {
                                ItemId = item.Id,
                                LanguageId = lang.Id
                            };
                            switch (lang.Id)
                            {
                                case 1:
                                    newLoc.LocName = matchingLoc.EnName;
                                    break;
                                case 2:
                                    newLoc.LocName = matchingLoc.RuName;
                                    break;
                            }
                            locItemToInsert.Add(newLoc);
                        }
                    }
                }
            }

            UpdateExistingLocs();
            InsertNewLoc();
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
                    Name = Convert.ToString(row[i++]),
                    Def = Convert.ToString(row[i++])
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

            Console.WriteLine($"Read {existingDBLocalizations.Count} Existing Localizations from DB");

        }

        private static void ImportDBLanguages()
        {
            string query = "SELECT language.id, language.name, language.shortname FROM language";

            var ds = sql.SelectDataSet(query);

            foreach (var row in ds)
            {
                int i = 0;
                var dbLanguage = new Language
                {
                    Id = Convert.ToInt32(row[i++]),
                    Name = Convert.ToString(row[i++]),
                    ShortName = Convert.ToString(row[i++])
                };
                languages.Add(dbLanguage);
            }
        }

        private static void ImportManualGatheringFile()
        {
            string jsonString = FileReader.ReadFile("Data\\LocNameImporter\\manualgathering.json");
            var manualLoc = JsonConvert.DeserializeObject<List<LocItem>>(jsonString);
            Console.WriteLine($"Read {manualLoc.Count} Items from Manual Gathering File");
            manualLocItems = manualLoc;
        }

        private static void UpdateExistingLocs()
        {
            foreach (var loc in locItemToUpdate)
            {
                var parameters = new List<Parameter> {
                    new Parameter{ Identifier = "locname", Value = loc.LocName },
                    new Parameter{ Identifier = "id", Value = loc.Id }
                };
                sql.ExecuteSQL("UPDATE itemlocalization SET itemlocalization.localizedname = @locname WHERE itemlocalization.id = @id", parameters);
            }
            Console.WriteLine($"Successfully updated {locItemToUpdate.Count} existing localizations");
        }

        private static void InsertNewLoc()
        {
            foreach (var loc in locItemToInsert)
            {
                var parameters = new List<Parameter> {
                    new Parameter{ Identifier = "languageid", Value = loc.LanguageId },
                    new Parameter{ Identifier = "itemid", Value = loc.ItemId },
                    new Parameter{ Identifier = "locname", Value = loc.LocName }
                };
                sql.ExecuteSQL("INSERT INTO itemlocalization (languagenumber, itemnumber, localizedname) VALUES (@languageid, @itemid, @locname);", parameters);
            }
            Console.WriteLine($"Successfully inserted {locItemToInsert.Count} new localizations");
        }
    }
}
