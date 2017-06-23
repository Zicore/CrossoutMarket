namespace Crossout.Data.Stats.Main
{
    public partial class PartStatsSummon : PartStatsBase
    {
        // Custom Properties
        [Stat("Structure", 60)]
        public double StatStructure
        {
            get { return health; }
        }
    }
}
