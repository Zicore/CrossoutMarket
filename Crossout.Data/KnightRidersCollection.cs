using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Crossout.Data.KnightRiders;
using NLog;
using Newtonsoft.Json;

namespace Crossout.Data
{
    public class KnightRidersCollection
    {
        private Logger Log = LogManager.GetCurrentClassLogger();

        public List<EventItem> EventItems { get; } = new List<EventItem>();
        
        public KnightRidersCollection()
        {
            
        }

        public void ReadPackages(string directory)
        {
            EventItem eventItem = new EventItem();
            DirectoryInfo packageDir = new DirectoryInfo(directory);

            foreach (var file in packageDir.GetFiles())
            {
                StreamReader sr = new StreamReader(file.FullName);

                string output = sr.ReadToEnd();

                eventItem = JsonConvert.DeserializeObject<EventItem>(output);
                EventItems.Add(eventItem);
            }
        }
    }
}
