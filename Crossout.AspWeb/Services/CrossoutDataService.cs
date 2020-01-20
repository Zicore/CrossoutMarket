using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Crossout.Data;
using Crossout.Data.Descriptions;
using Crossout.Data.Stats;
using Crossout.Data.Stats.Main;
using Crossout.Model.Items;
using Microsoft.AspNetCore.Hosting;
using NLog;

namespace Crossout.AspWeb.Services
{
    public class CrossoutDataService
    {
        private Logger Log = LogManager.GetCurrentClassLogger();
        private string replaceColorPattern = @"(?<start>[^@]+)@(?<color>[0-9a-f]{8})(?<value>[^@]+)(?<end>.*)";
        private string replaceValuesPattern = @"(?<start>[^\$]+)\$(?<key>[^\$]+)\$(?<end>.*)";
        private readonly Regex replaceValuesRegex;
        private readonly Regex replaceColorRegex;
        private CrossoutDataService()
        {
            replaceValuesRegex = new Regex(replaceValuesPattern);
            replaceColorRegex = new Regex(replaceColorPattern, RegexOptions.IgnoreCase);
        }

        public PartStatsCollection CoreStatsCollection { get; } = new PartStatsCollection();
        public PartStatsCollection CabinStatsCollection { get; } = new PartStatsCollection();
        public PartStatsCollection DecorumStatsCollection { get; } = new PartStatsCollection();
        public PartStatsCollection WeaponStatsCollection { get; } = new PartStatsCollection();
        public PartStatsCollection MovementStatsCollection { get; } = new PartStatsCollection();
        public ReverseItemLookup ReverseItemLookup { get; } = new ReverseItemLookup();
        public StringLookup StringLookup { get; } = new StringLookup();
        public PremiumPackagesColletion PremiumPackagesCollection { get; } = new PremiumPackagesColletion();
        public KnightRidersCollection KnightRidersCollection { get; } = new KnightRidersCollection();

        public static void Initialize(IWebHostEnvironment webHostEnvironment)
        {
            _instance = new CrossoutDataService();
            _instance.LoadFiles(webHostEnvironment);
        }

        private void LoadFiles(IWebHostEnvironment webHostEnvironment)
        {
            var rootPath = webHostEnvironment.ContentRootPath;
            ReverseItemLookup.ReadStats(Path.Combine(rootPath, WebSettings.Settings.FileStringsEnglish));
            StringLookup.ReadStats(Path.Combine(rootPath, WebSettings.Settings.FileStringsEnglish));

            CabinStatsCollection.ReadStats<PartStatsCabin>(Path.Combine(rootPath, WebSettings.Settings.FileCarEditorCabinsLua));
            DecorumStatsCollection.ReadStats<PartStatsDecor>(Path.Combine(rootPath, WebSettings.Settings.FileCarEditorDecorumLua));
            WeaponStatsCollection.ReadStats<PartStatsWeapon>(Path.Combine(rootPath, WebSettings.Settings.FileCarEditorWeaponsExLua));
            MovementStatsCollection.ReadStats<PartStatsWheel>(Path.Combine(rootPath, WebSettings.Settings.FileCarEditorWheelsLua));
            CoreStatsCollection.ReadStats<PartStatsCore>(Path.Combine(rootPath, WebSettings.Settings.FileCarEditorCoreLua));

            PremiumPackagesCollection.ReadPackages(Path.Combine(rootPath, WebSettings.Settings.DirectoryPremiumPackages));
            KnightRidersCollection.ReadPackages(Path.Combine(rootPath, WebSettings.Settings.DirectoryKnightRiders));
        }

        public PartStatsBase Get(string internalKey, PartStatsCollection statsCollection)
        {
            if (ReverseItemLookup.Items.ContainsKey(internalKey))
            {
                var key = ReverseItemLookup.Items[internalKey].ToLowerInvariant();
                if (statsCollection.Items.ContainsKey(key))
                {
                    return statsCollection.Items[key];
                }
            }
            return null;
        }

        public string GetKey(string internalKey)
        {
            if (ReverseItemLookup.Items.ContainsKey(internalKey))
            {
                var key = ReverseItemLookup.Items[internalKey].ToLowerInvariant();
                return key;
            }
            return null;
        }

        

        public void AddData(Item item)
        {
            AddStats(item);
            AddDescription(item);
            ReplaceValues(item);
        }

        private void AddStats(Item item)
        {
            //1	Frame
            //2	Weapons
            //3	Hardware
            //4	Movement
            //5	Structure
            //6	Decor
            //7	Dyes
            //8	Resources

            const int CategoryFrame = 1;
            const int CategoryWeapon = 2;
            const int CategoryHardware = 3;
            const int CategoryMovement = 4;
            const int CategoryStructure = 5;
            const int CategoryDecor = 6;
            const int CategoryDyes = 7;
            const int CategoryResources = 8;

            if (item.CategoryId == CategoryFrame)
            {
                item.Stats = Get(item.Name, CabinStatsCollection);
            }

            if (item.CategoryId == CategoryWeapon) // Rewrite with better lookup to avoid magic values.
            {
                item.Stats = Get(item.Name, WeaponStatsCollection);
            }

            if (item.CategoryId == CategoryHardware)
            {
                item.Stats = Get(item.Name, CoreStatsCollection);
            }

            if (item.CategoryId == CategoryMovement)
            {
                item.Stats = Get(item.Name, MovementStatsCollection);
            }

            if (item.CategoryId == CategoryDecor)
            {
                item.Stats = Get(item.Name, DecorumStatsCollection);
            }
        }

        private void AddDescription(Item item)
        {
            var key = GetKey(item.Name);
            if (key != null)
            {
                var desc = StringLookup.ReadDescription(key);
                if(desc != null)
                {
                    desc = ReplaceNewLines(desc);
                    item.Description = new ItemDescription { Text = desc };
                }
            }
        }



        public static string ReplaceNewLines(string description)
        {
            string result = description;
            result = result.Replace("|", "<br>");
            return result;
        }

        private void ReplaceValues(Item item)
        {
            if (item.Description != null)
            {
                string result = item.Description.Text;

                if (item.Stats != null)
                {
                    Match match;

                    do
                    {
                        match = replaceValuesRegex.Match(result);
                        if (match.Success)
                        {
                            var key = match.Groups["key"].Value;
                            
                            if (item.Stats.Fields.ContainsKey(key))
                            {
                                var value = item.Stats.Fields[key];

                                var stat = item.Stats.SortedStats.FirstOrDefault(x => x.Stat.OverrideDescriptionStat != null && x.Stat.OverrideDescriptionStat.Equals(key));
                                if(stat != null)
                                {
                                    value = stat.Value;
                                }

                                result = Regex.Replace(result, replaceValuesPattern, $"${{start}}{value}${{end}}");
                            }
                            else
                            {
                                //Log.Warn($"Couldn't replace description value. Item: {item.Name} Key: {key}");
                                break;
                            }
                            
                        }
                    } while (match.Success);
                }

                Match matchColor;

                do
                {
                    matchColor = replaceColorRegex.Match(result);

                    if (matchColor.Success)
                    {
                        var color = matchColor.Groups["color"].Value;
                        var value = matchColor.Groups["value"].Value;

                        if (!color.Equals("ffffffff", StringComparison.CurrentCultureIgnoreCase))
                        {
                            result = Regex.Replace(result, replaceColorPattern, $"${{start}}<span class='{ColorToClass(color)}'>{value}</span>${{end}}", RegexOptions.IgnoreCase);
                        }
                        else
                        {
                            result = Regex.Replace(result, replaceColorPattern,$"${{start}}{value}${{end}}", RegexOptions.IgnoreCase);
                        }
                    }
                } while (matchColor.Success);
                

                item.Description.Text = result;
            }
        }

        private string ColorToClass(string color)
        {
            if (color.Equals("ffff0000", StringComparison.InvariantCultureIgnoreCase))
            {
                return "desc-red";
            }

            if (color.Equals("ff00ff00", StringComparison.InvariantCultureIgnoreCase))
            {
                return "desc-green";
            }
            return "desc-default";
        }

        private List<int> GetAppIDList()
        {
            List<int> list = new List<int>();
            foreach(var package in PremiumPackagesCollection.Packages)
            {
                list.Add(package.SteamAppID);
            }
            return list;
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
