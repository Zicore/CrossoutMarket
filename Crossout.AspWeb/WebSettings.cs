using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NLog;
using Zicore.Settings.Json;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.AspWeb
{
    public class WebSettings : JsonSerializable
    {
        public string CurrentVersion { get; set; } = "0.7.0"; // For Folder

        public static WebSettings Settings = new WebSettings { JsonSerializerSettings = new JsonSerializerSettings {Formatting = Formatting.Indented}, CreateSubdirectoryIfItNotExists = true};

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string DatabaseName { get; set; } = "";
        public string DatabaseHost { get; set; } = "";
        public string DatabasePassword { get; set; } = "";
        public string DatabaseUsername { get; set; } = "";
        public int DatabasePort { get; set; } = 3306;
        public string SignalrHost { get; set; } = "localhost";
        public int WebserverPort { get; set; } = 80;
        public string DataHost { get; set; } = "localhost";

        public string GoogleConsumerKey { get; set; } = "";
        public string GoogleConsumerSecret { get; set; } = "";
        public bool EnableAds { get; set; } = false;

        public string FileCarEditorWeaponsExLua { get; set; } = @"Resources\Data\0.7.0\gamedata\def\ex\car_editor_weapons_ex.lua";
        public string FileCarEditorCabinsLua { get; set; } = @"Resources\Data\0.7.0\gamedata\def\ex\car_editor_cabins.lua";
        public string FileCarEditorDecorumLua { get; set; } = @"Resources\Data\0.7.0\gamedata\def\ex\car_editor_decorum.lua";
        public string FileCarEditorWheelsLua { get; set; } = @"Resources\Data\0.7.0\gamedata\def\ex\car_editor_wheels.lua";
        public string FileCarEditorCoreLua { get; set; } = @"Resources\Data\0.7.0\gamedata\def\ex\car_editor_core.lua";
        public string FileStringsEnglish { get; set; } = @"Resources\Data\0.7.0\strings\english\string.txt";

        public string DirectoryPremiumPackages { get; set; } = @"Resources\PremiumPackages";
        public string DirectoryKnightRiders { get; set; } = @"Resources\KnightRiders";

        public static string Title => "Crossout DB - Crossout Market and Crafting Calculator";

        public void Load()
        {
            try
            {
                LoadFromAppData(FileName, ApplicationName);
            }
            catch (FileNotFoundException)
            {
                SaveToAppData(FileName, ApplicationName);
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
        }

        public void Save()
        {
            SaveToAppData(FileName, ApplicationName);
        }

        public ConnectorDescription CreateDescription()
        {
            return new ConnectorDescription
            {
                Database = DatabaseName,
                Host = DatabaseHost,
                Password = DatabasePassword,
                Username = DatabaseUsername,
                Port = DatabasePort
            };
        }

        public static string ApplicationName = "CrossoutWeb";
        public static string FileName = "WebSettings.json";
        
    }
}
