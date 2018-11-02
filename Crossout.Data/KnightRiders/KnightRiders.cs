using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Data.KnightRiders
{
    public class Ingredient
    {
        public string Name;
        public int Id;
        public int Amount;
        public decimal SellPrice;
        public decimal BuyPrice;
        public string FormatSellPrice;
        public string FormatBuyPrice;
    }

    public class EventItem
    {
        public string Key;
        public string Name;
        public decimal SellPrice;
        public decimal BuyPrice;
        public string FormatSellPrice;
        public string FormatBuyPrice;
        public int Talers;
        public int? Id;
        public List<Ingredient> Ingredients;
        public string FormatSellSum;
        public string FormatBuySum;
        public decimal TotalSellSum;
        public decimal TotalBuySum;
        public string FormatTotalSellSum;
        public string FormatTotalBuySum;
    }
}
