namespace Crossout.Data.Stats.Main
{
    public partial class PartStatsDecor : PartStatsBase
    {
        // Custom Properties

        [Stat("Power Score", 0, CustomClasses = StatAttribute.PowerScoreClasses)]
        public int StatPowerScore
        {
            get { return universal_rating; }
        }

        //------

        [Stat("Structure", 70)]
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
