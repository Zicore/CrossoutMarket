namespace Crossout.Data.Stats.Main
{
    public partial class PartStatsWheel : PartStatsBase
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

        [Stat("Max. Speed", 10)]
        public double StatMaxSpeed
        {
            get { return speed_limit; }
        }

        [Stat("Tonnage", 10)]
        public double StatTonnageAdd
        {
            get { return tonnage_add; }
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
