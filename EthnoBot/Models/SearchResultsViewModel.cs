using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class SearchResultsViewModel
    {

        public List<Product> products { get; set; }
       
        public List<Producer> producers { get; set; }
        public List<Category> categories { get; set; }
    }
}