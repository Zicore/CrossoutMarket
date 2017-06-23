namespace Crossout.Data.Stats.Main
{
    partial class PartStatsCabin : PartStatsBase
    {
        // Custom Properties

        [Stat("Power Score", 0, CustomClasses = StatAttribute.PowerScoreClasses)]
        public int StatPowerScore
        {
            get { return universal_rating; }
        }

        [Stat("Required Level", 5)]
        public int StatRequiredLevel
        {
            get { return require_faction_level; }
        }

        //------

        [Stat("Engine Speed", 10)]
        public int StatEngineSpeed
        {
            get { return engine_max_speed; }
        }

        [Stat("Max. Speed", 20)]
        public int StatMaxSpeed
        {
            get { return engine_max_speed; }
        }

        [Stat("Power", 20)]
        public int StatEnginePower
        {
            get { return engine_power; }
        }

        [Stat("Tonnage", 20)]
        public int StatTonnage
        {
            get { return tonnage; }
        }

        [Stat("Power", 20)]
        public int StatEnginePower
        {
            get { return engine_power; }
        }

        [Stat("Power", 20)]
        public int StatEnginePower
        {
            get { return engine_power; }
        }

        //------

        [Stat("Structure", 60)]
        public double StatStructure
        {
            get { return health; }
        }

        [Stat("Mass", 80)]
        public double StatMass
        {
            get { return physics_mass; }
        }
    }
}
