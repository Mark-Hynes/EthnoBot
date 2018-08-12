using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class CartItemViewModel
    {
        public CartItem CartItem { get; set; }
        public Seller Seller { get; set; }
        public Product Product { get; set; }
        public Listing Listing { get; set; }
        public float Subtotal { get; set; }
    }
}