namespace Crossout.Data.Stats.Main
{
    public class SingleStat
    {
        public string Key { get; set; }
        public StatAttribute Stat { get; set; }
        public object Value { get; set; }

        public bool DisplayValue
        {
            get { return Value != null && !Value.Equals(0.0) && !Value.Equals(0); }
        }
    }
}