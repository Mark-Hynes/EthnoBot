using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    [Table("ProducerProducts")]
    public class ProducerProduct
    {
        [Key]
        public int ProducerProductId { get; set; }
        public int ProducerId { get; set; }
        public int ProductId { get; set; }
        public double Price { get; set; }

    }
}