﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
 
        [Table("Offers")]
    public class Offer
    {
        [Key]
            public string OfferId { get; set; }

            [Column]
            public string ListingId { get; set; }
            [Column]
            public Decimal Price { get; set; }
            [Column]
            public string Currency { get; set; }
            [Column]
            public Decimal Units { get; set; }
            [Column]
            public string Measurement { get; set; }

    }
    }