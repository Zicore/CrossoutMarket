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
            // This tool is used to generate all the properties

            string basePath = @"\Data\0.7.0\gamedata\def\ex";
            string weapons = "car_editor_weapons_ex.lua";
            string wheels = "car_editor_wheels.lua";
            string core = "car_editor_core.lua";

            string path = Path.Combine(basePath, core);

            StatsReader reader = new StatsReader();
            reader.ReadFields(path);
        }
    }
}
