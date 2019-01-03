using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class ManageSellerViewModel
    {
        public Seller Seller { get; set; }
        public List<ListingViewModel> listingViewModels { get; set; }
    }
}