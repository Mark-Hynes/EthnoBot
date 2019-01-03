using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class OrderViewModel
    {
        public Order Order;
        public Seller Seller;
        public ApplicationUser Buyer;
        public Product Product;
    }
}