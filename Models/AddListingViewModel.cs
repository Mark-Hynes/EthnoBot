using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class AddListingViewModel
    {
        public List<ListingTag> ListingTags;
        public List<ListingTagCategory> ListingTagCategories;
        public string ImagePath;
        public List<Product> Products;
    }
}