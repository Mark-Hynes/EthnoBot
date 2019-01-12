using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthnoBot.Models
{
    public class ModifyProductViewModel
    {
        public string ProductId { get; set; }
        public Product Product {get;set;}
        public List<Tag> CurrentProductTags { get; set; }
        
        
        

    }
}