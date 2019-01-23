using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthnoBot.Models
{
   
    [Table("Products")]
    public class Product
    {
        [Key]
        [ScaffoldColumn(false)]
        public string ProductId { get; set; }
       
        [DisplayName("Product Name")]
        [Required(ErrorMessage = "Product Name is required.")]
        public string Title { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Abstract")]
        public string Abstract { get; set; }

        [DisplayName("Product Image")]
        public string ImagePath { get; set; }
        [DisplayName("Product Type")]
        public string ProductType { get; set; }


    }
}