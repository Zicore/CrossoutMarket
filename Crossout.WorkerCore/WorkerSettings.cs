using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NLog;
using Zicore.Settings.Json;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.WorkerCore
{
    public class WorkerSettings : JsonSerializable
    {
        public static WorkerSettings Settings = new WorkerSettings { JsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented }, CreateSubdirectoryIfItNotExists = true };

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string DatabaseName { get; set; } = "";
        public string DatabaseHost { get; set; } = "";
        public string DatabasePassword { get; set; } = "";
        public string DatabaseUsername { get; set; } = "";
        public int DatabasePort { get; set; } = 3306;

        public List<TaskSettings> TaskSettings { get; set; } = new List<TaskSettings>();

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

        public static string ApplicationName = "CrossoutWorker";
        public static string FileName = "WorkerSettings.json";
       
    }
}
