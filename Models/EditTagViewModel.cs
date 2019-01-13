using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class EditTagViewModel
    {
        public Tag Tag { get; set; }
        public TagCategory TagCategory { get; set; }

        public List<TagCategory> TagCategories { get; set; }
    }
}