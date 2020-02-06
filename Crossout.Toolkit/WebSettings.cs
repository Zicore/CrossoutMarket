using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Zicore.Settings.Json;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Toolkit
{
    public class ToolkitSettings : JsonSerializable
    {
        public static ToolkitSettings Settings = new ToolkitSettings { JsonSerializerSettings = new JsonSerializerSettings {Formatting = Formatting.Indented}, CreateSubdirectoryIfItNotExists = true};

        public string DatabaseName { get; set; } = "";
        public string DatabaseHost { get; set; } = "";
        public string DatabasePassword { get; set; } = "";
        public string DatabaseUsername { get; set; } = "";
        public int DatabasePort { get; set; } = 3306;

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
                Console.WriteLine(ex);
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

        public static string ApplicationName = "CrossoutToolkit";
        public static string FileName = "ToolkitSettings.json";
        
    }
}
