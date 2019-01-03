
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EthnoBot.Models
{
    [Table("Carts")]
    public class Cart
    {
        [Key]
        public string CartId { get; set; }

        public string UserId { get; set; }


    }
}