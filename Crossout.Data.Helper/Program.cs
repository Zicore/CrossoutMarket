using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Data.Helper
{
    class Program
    {
        static void Main(string[] args)
        {
            // This tool is used to generate all the property classes from game files

            string versionFolder = "0.9.120.87835";

            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Resources", "Data", versionFolder, "gamedata", "def", "ex");
            
            List<GeneratorDescription> descriptions = new List<GeneratorDescription>
            {
                new GeneratorDescription { Classname = "PartStatsWeapon", SourceFileName = "car_editor_weapons_ex.lua", ClassFileName = "PartStatsWeapon.Generated.cs"},
                new GeneratorDescription { Classname = "PartStatsCabin", SourceFileName = "car_editor_cabins.lua", ClassFileName = "PartStatsCabin.Generated.cs"},
                new GeneratorDescription { Classname = "PartStatsDecor", SourceFileName = "car_editor_decorum.lua", ClassFileName = "PartStatsDecor.Generated.cs"},
                new GeneratorDescription { Classname = "PartStatsMelee", SourceFileName = "car_editor_melee.lua", ClassFileName = "PartStatsMelee.Generated.cs"},
                new GeneratorDescription { Classname = "PartStatsStructure", SourceFileName = "car_editor_structure.lua", ClassFileName = "PartStatsStructure.Generated.cs"},
                new GeneratorDescription { Classname = "PartStatsSummon", SourceFileName = "car_editor_summons.lua", ClassFileName = "PartStatsSummon.Generated.cs"},
                new GeneratorDescription { Classname = "PartStatsWheel", SourceFileName = "car_editor_wheels.lua", ClassFileName = "PartStatsWheel.Generated.cs"},
                new GeneratorDescription { Classname = "PartStatsCore", SourceFileName = "car_editor_core.lua", ClassFileName = "PartStatsCore.Generated.cs"},
            };

            var destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Result");

            StatsGenerator gen = new StatsGenerator { SourceFileDirectory = basePath,DestinationFileDirectory = destinationPath };
            gen.Generate(descriptions);
        }
    }
}
