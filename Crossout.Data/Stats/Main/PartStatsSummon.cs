namespace Crossout.Data.Stats.Main
{
    public partial class PartStatsSummon : PartStatsBase
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

        //------

        [Stat("Structure", 60)]
        public double StatStructure
        {
            get { return health; }
        }

        [Stat("Energy Drain", 70)]
        public int StatEnergyDrain
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
