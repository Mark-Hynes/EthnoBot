using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class ManageProducerViewModel
    {
        public Producer producer { get; set; }
        public List<ListingInfo> listings { get; set; }
    }
}