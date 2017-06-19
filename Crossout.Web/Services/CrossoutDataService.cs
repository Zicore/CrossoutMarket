using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Data;
using Crossout.Model.Items;

namespace Crossout.Web.Services
{
    public class CrossoutDataService
    {
        private CrossoutDataService()
        {

        }

        public PartStatsCollection StatsCollection { get; } = new PartStatsCollection();
        public ReverseItemLookup ReverseItemLookup { get; } = new ReverseItemLookup();

        public static void Initialize()
        {
            _instance = new CrossoutDataService();
            _instance.LoadFiles();
        }

        private void LoadFiles()
        {
            var rootPath = RootPathProvider.GetRootPathStatic();
            ReverseItemLookup.ReadStats(Path.Combine(rootPath, WebSettings.Settings.FileStringsEnglish));
            StatsCollection.ReadStats(Path.Combine(rootPath, WebSettings.Settings.FileCarEditorWeaponsExLua));
        }

        public PartStats Get(string internalKey)
        {
            if (ReverseItemLookup.Items.ContainsKey(internalKey))
            {
                var key = ReverseItemLookup.Items[internalKey];
                if (StatsCollection.Items.ContainsKey(key))
                {
                    return StatsCollection.Items[key];
                }
            }
            return null;
        }

        public void AddStats(Item item)
        {
            var stats = Get(item.Name);
            item.Stats = stats;
        }

        private static CrossoutDataService _instance;

        public static CrossoutDataService Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
