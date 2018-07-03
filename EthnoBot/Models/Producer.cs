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
    [Bind(Exclude = "ProducerId")]
    [Table("Producers")]
    public class Producer
    {
        [Key]
        [ScaffoldColumn(false)]
        public int ProducerId { get; set; }
        [DisplayName("Retailer Name")]
        [Required(ErrorMessage = "Retailer Name is required.")]
        public string Name { get; set; }
        [DisplayName("Retailer Description")]
        [Required(ErrorMessage = "Retailer Description is required.")]
        public string Description { get; set; }
        [DisplayName("About Retailer")]
        public string About { get; set; }
        [DisplayName("Retailer Address")]
        [Required(ErrorMessage = "Retailer Address is required.")]
        public string Address { get; set; }
        [DisplayName("Quality of Service Rating")]
        public int AverageRating { get; set; }
        [DisplayName("Mobile Number")]

        public string Mobile { get; set; }
        [DisplayName("Telephone Number")]
        [Required(ErrorMessage = "Telephone Number is required.")]
        public string Telephone { get; set; }
        [DisplayName("Company Email Address")]
        [Required(ErrorMessage = "Company Email Address is required.")]
        public string CompanyEmail { get; set; }
        [DisplayName("Customer Service Address")]
        [Required(ErrorMessage = "Customer Service Email Address is required.")]
        public string CustomerServiceEmail { get; set; }
        [DisplayName("Image Path")]

        public string ImagePath { get; set; }
        public string ASPUserId{ get; set; }
        public string Verified { get; set; }
        
    }
}