namespace Crossout.Data.Stats.Main
{
    public partial class PartStatsCore : PartStatsBase
    {
        // Custom Properties

        [Stat("Power Score", 0, CustomClasses = StatAttribute.PowerScoreClasses)]
        public int StatPowerScore
        {
            get { return universal_rating; }
        }

        //------

        [Stat("Adds Energy", 10)]
        public double StatPower
        {
            get { return power; }
        }

        [Stat("Engine Speed", 10)]
        public double StatEngineSpeed
        {
            get { return engine_max_speed; }
        }

        [Stat("Power", 10)]
        public double StatEnginePower
        {
            get { return engine_power; }
        }

        [Stat("Mass Limit", 10)]
        public double StatMassLimit
        {
            get { return critical_mass_add; }
        }

        //------

        [Stat("Structure", 60)]
        public double StatStructure
        {
            get { return health; }
        }

        [Stat("Energy Drain", 70)]
        public double StatEnergyDrain
        {
            get { return power_require; }
        }

        [Stat("Mass", 80)]
        public double StatMass
        {
            get { return physics_mass; }
        }
    }
}
