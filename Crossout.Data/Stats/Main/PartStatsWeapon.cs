namespace Crossout.Data.Stats.Main
{
    public partial class PartStatsWeapon : PartStatsBase
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

        [Stat("Damage Rating", 10, ShowProgressBar = true)]
        public double StatPercentDamageRating
        {
            get { return damage_rating * 100.0f; }
        }

        [Stat("Fire Rate Rating", 20, ShowProgressBar = true)]
        public double StatPercentFireRateRating
        {
            get { return fire_rate_rating * 100.0f; }
        }

        [Stat("Range Rating", 30, ShowProgressBar = true)]
        public double StatPercentRangeRating
        {
            get { return range_rating * 100.0f; }
        }

        [Stat("Accuracy Rating", 40, ShowProgressBar = true)]
        public double StatPercentAccuracyRating
        {
            get { return accuracy_rating * 100.0f; }
        }

        [Stat("Overheat Rating", 50, ShowProgressBar = true)]
        public double StatPercentOverheatRating
        {
            get { return overheat_rating * 100.0f; }
        }

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
