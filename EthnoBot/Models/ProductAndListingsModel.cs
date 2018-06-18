using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class ProductAndListingsModel
    {

        public Product product { get; set; }
     public List<ProducerProduct> producerProducts { get; set; }
}
}