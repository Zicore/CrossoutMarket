using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Worker.Models.MarketGrouper
{
    public class MarketItem
    {
        public int Id;
        public int ItemNumber;
        public int SellPrice;
        public int BuyPrice;
        public int SellOffers;
        public int BuyOrders;
        public DateTime DateTime;
    }
}
