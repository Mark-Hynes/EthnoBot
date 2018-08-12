using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class CartItem
    {
        public string CartItemId { get; set; }
        public string SellerId { get; set; }
        public string CartId { get; set; }
        public string ProductId { get; set; }
        public float UnitsKG { get; set; }
        public float UnitPriceKG { get; set; }  
    }
}


