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
        public float UnitPriceKG;
        public float UnitsKG;
        public ListingViewModel()
        { }
    }
}