using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class FeatureSearchItem
    {
        [Key]
        public string FeatureSearchItemId { get; set; }
        public string ImagePath { get; set; }
        public string DisplayedText { get; set; }
        public string SearchText { get; set; }
        public string LinkText { get; set; }
        public string Type { get; set; }
    }
}