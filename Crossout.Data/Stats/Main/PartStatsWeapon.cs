namespace Crossout.Data.Stats.Main
{
    public partial class PartStatsWeapon : PartStatsBase
    {
        // Custom Properties
        [Stat("Damage Rating", 10)]
        public double StatPercentDamageRating
        {
            get { return damage_rating * 100.0f; }
        }

        [Stat("Fire Rate Rating", 10)]
        public double StatPercentFireRateRating
        {
            get { return fire_rate_rating * 100.0f; }
        }

        [Stat("Range Rating", 10)]
        public double StatPercentRangeRating
        {
            get { return range_rating * 100.0f; }
        }

        [Stat("Accuracy Rating", 10)]
        public double StatPercentAccuracyRating
        {
            get { return accuracy_rating * 100.0f; }
        }

        [Stat("Overheat Rating", 10)]
        public double StatPercentOverheatRating
        {
            get { return overheat_rating * 100.0f; }
        }

        //public double fire_rate_rating { get; set; }
        //public double range_rating { get; set; }
        //public double accuracy_rating { get; set; }
        //public double overheat_rating { get; set; }
    }
}
