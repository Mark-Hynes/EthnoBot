﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthnoBot.Models
{
    [Bind(Exclude = "ProductId")]
    [Table("Products")]
    public class Product
    {
        [Key]
        [ScaffoldColumn(false)]
        public int ProductId { get; set; }
        [DisplayName("Product Category")]
        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }
        [DisplayName("Product Name")]
        [Required(ErrorMessage = "Product Name is required.")]
        public string Title { get; set; }
        [DisplayName("Latin Name")]

        public string LatinName { get; set; }
        [DisplayName("Image Link")]
        public string ItemArtURL { get; set; }
        [DisplayName("Product Family")]
        public string Family { get; set; }


    }
}