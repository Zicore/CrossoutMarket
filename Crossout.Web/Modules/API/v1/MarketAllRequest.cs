namespace Crossout.Web.Modules.API.v1
{
    public class MarketAllRequest
    {
        public int Id { get; set; }

        public int? StartTimestamp { get; set; }

        public int? EndTimestamp { get; set; }
    }
}
