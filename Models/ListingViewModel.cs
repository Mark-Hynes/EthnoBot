using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class ListingViewModel
    {
        public Listing Listing;
        public Product Product;
        public Seller Seller;
        public List<Offer> Offers;
        
        public ListingViewModel()
        { }
    }
}