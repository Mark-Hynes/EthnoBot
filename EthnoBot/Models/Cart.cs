
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EthnoBot.Models
{
    [Table("Carts")]
    public class Cart
    {
        [Key]
        public string CartID { get; set; }

        public string UserID { get; set; }

        public string CartItems { get; set; }
    }
}