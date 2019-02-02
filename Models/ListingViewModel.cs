using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class ListingViewModel
    {
        public List<Product> Products;
        public Listing Listing;
        public Product Product;
        public Seller Seller;
        public List<Offer> Offers;
        public Offer NewOffer;
        public Tag ProcessingOption;
        public ListingViewModel()
        { }

        public List<ListingTagCategory> ListingTagCategories { get; internal set; }
        public List<ListingTag> ListingTags { get; internal set; }
    }
}