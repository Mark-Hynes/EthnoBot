using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class Order
    {
        [Key]
        public string OrderItemId { get; set; }
        public string OrderId { get; set; }

        public string ProductId { get; set; }
        public string SellerId { get; set; }
        public string BuyerId { get; set; }
        public decimal UnitsKG { get; set; }
        public decimal UnitPriceKG { get; set; }
        public decimal TotalPrice { get; set; }
       
        public string Timestamp { get; set; }

        public int Status { get; set; }
    }
}