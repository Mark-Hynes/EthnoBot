using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class ProductAndListingsModel
    {
        public Product Product; 
        public List<ListingViewModel> ListingViewModels;
        public List<ListingTag> processingOptions;
        public List<Tag> ProductTags;
        public List<TagCategory> TagCategories;



    }
}