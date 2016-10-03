using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using Zicore.Settings.Json;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web
{
    public class WebSettings : JsonSettings
    {
        public static WebSettings Settings = new WebSettings();

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string DatabaseName { get; set; } = "";
        public string DatabaseHost { get; set; } = "";
        public string DatabasePassword { get; set; } = "";
        public string DatabaseUsername { get; set; } = "";
        public int DatabasePort { get; set; } = 3306;
        public string SignalrHost { get; set; } = "localhost";
        public int WebserverPort { get; set; } = 80;
        public string DataHost { get; set; } = "localhost";

        public static string Title => "Crossout DB";

        public void Load()
        {
            try
            {
                Load(ApplicationName, FileName);
            }
            catch (FileNotFoundException)
            {
                Save();
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
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
