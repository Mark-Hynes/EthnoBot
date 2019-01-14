using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class FeatureSearchItemViewModel
    {
        public FeatureSearchItem FeatureSearchItem { get; set; }
        public List<Product> Products { get; set; }
    }
}