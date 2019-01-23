using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    [Table("Listings")]
    public class Listing
    {
        [Key]
        public string ListingId { get; set; }
        public string SellerId { get; set; }
        public string ProductId { get; set; }
        public string Currency { get; set; }
        public string Measurement { get; set; }
        public decimal Units { get; set; }
        public decimal Price { get; set; }

    }
}