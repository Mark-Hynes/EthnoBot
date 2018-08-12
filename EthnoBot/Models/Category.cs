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
    [Table("Categories")]
    public class Category
    {
        [Key]
        public string CategoryId { get; set; }
        [DisplayName("Category Name")]
        [Required(ErrorMessage = "Category Name is required.")]
        public string Name { get; set; }
        [DisplayName("Category Description")]
        [Required(ErrorMessage = "Category Description is required.")]
        public string Description { get; set; }

        [DisplayName("Image Path")]
        public string ImagePath { get; set; }

      
    }
}