using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Data.Descriptions;
using Crossout.Data.Stats.Main;
using Crossout.Model.Formatter;
using Newtonsoft.Json;

namespace Crossout.Web.Models.API.v1
{
    public class ApiItemEntry : ApiEntryBase
    {

        [JsonIgnore]
        public PartStatsBase Stats { get; set; }

        [JsonIgnore]
        public ItemDescription Description { get; set; }
        
        [JsonProperty("selloffers")]
        public int SellOffers { get; set; }

        [JsonProperty("sellprice")]
        public decimal SellPrice { get; set; }

        [JsonProperty("buyorders")]
        public int BuyOrders { get; set; }

        [JsonProperty("buyprice")]
        public decimal BuyPrice { get; set; }

        [JsonProperty("removed")]
        public int Removed { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }

        [JsonProperty("workbenchrarity")]
        public int WorkbenchRarity { get; set; }

        [JsonProperty("margin")]
        public decimal Margin
        {
            get { return (decimal)(SellPrice - BuyPrice - (SellPrice * 0.1m)); }
        }

        [JsonProperty("formatmargin")]
        public string FormatMargin
        {
            get { return PriceFormatter.FormatPrice(Margin); }
        }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("lastupdatetime")]
        public string LastUpdateTime => Timestamp.ToString("yyyy-MM-dd HH:mm:ss");

        [JsonProperty("rarityname")]
        public int RarityId { get; set; }

        [JsonProperty("rarityname")]
        public string RarityName { get; set; }

        [JsonProperty("categoryid")]
        public int CategoryId { get; set; }

        [JsonProperty("categoryname")]
        public string CategoryName { get; set; }

        [JsonProperty("typeid")]
        public int TypeId { get; set; }

        [JsonProperty("recipeid")]
        public int RecipeId { get; set; }

        [JsonProperty("typename")]
        public string TypeName { get; set; }

        [JsonProperty("factionnumber")]
        public int FactionNumber { get; set; }

        [JsonProperty("faction")]
        public string Faction { get; set; }

        public bool OlderThan(int minutes)
        {
            return DateTime.Now - Timestamp > new TimeSpan(0, minutes, 0);
        }

        [JsonProperty("formatbuyprice")]
        public string FormatBuyPrice
        {
            get
            {
                return PriceFormatter.FormatPrice(BuyPrice);
            }
        }

        [JsonProperty("formatsellprice")]
        public string FormatSellPrice
        {
            get
            {
                return PriceFormatter.FormatPrice(SellPrice);
            }
        }

        [JsonProperty("image")]
        public string Image
        {
            get { return $"{Id}.png"; }
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(SellOffers)}: {SellOffers}, {nameof(SellPrice)}: {SellPrice}, {nameof(BuyOrders)}: {BuyOrders}, {nameof(BuyPrice)}: {BuyPrice}";
        }
    }
}
