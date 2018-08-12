using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    [Table("CartItems")]
    public class CartItem
    {
        [Key]
        public string CartItemId { get; set; }

        [Column]
        public string CartId { get; set; }
        [Column]
        public string ListingId { get; set; }
        [Column]
        public decimal UnitsKG { get; set; }
    
    
    }
}


