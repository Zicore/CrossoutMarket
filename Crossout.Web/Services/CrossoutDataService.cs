using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Data;
using Crossout.Data.Stats;
using Crossout.Model.Items;

namespace Crossout.Web.Services
{
    public class CrossoutDataService
    {
        private CrossoutDataService()
        {

        }

        public PartStatsCollection CoreStatsCollection { get; } = new PartStatsCollection();
        public PartStatsCollection WeaponStatsCollection { get; } = new PartStatsCollection();
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
            WeaponStatsCollection.ReadStats<PartStatsWeapon>(Path.Combine(rootPath, WebSettings.Settings.FileCarEditorWeaponsExLua));
            CoreStatsCollection.ReadStats<PartStatsCore>(Path.Combine(rootPath, WebSettings.Settings.FileCarEditorCoreLua));
        }

        public PartStatsBase Get(string internalKey, PartStatsCollection statsCollection)
        {
            if (ReverseItemLookup.Items.ContainsKey(internalKey))
            {
                var key = ReverseItemLookup.Items[internalKey];
                if (statsCollection.Items.ContainsKey(key))
                {
                    return statsCollection.Items[key];
                }
            }
            return null;
        }

        //1	Frame
        //2	Weapons
        //3	Hardware
        //4	Movement
        //5	Structure
        //6	Decor
        //7	Dyes
        //8	Resources

        public void AddStats(Item item)
        {
            const int CategoryWeapon = 2;
            const int CategoryHardware = 3;
            const int CategoryMovement = 4;

            if (item.CategoryId == CategoryWeapon) // Rewrite with better lookup to avoid magic values.
            {
                item.Stats = Get(item.Name, WeaponStatsCollection); // TODO: Decide what Stats
            }

            if (item.CategoryId == CategoryHardware)
            {
                item.Stats = Get(item.Name, CoreStatsCollection); // TODO: Decide what Stats
            }
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
