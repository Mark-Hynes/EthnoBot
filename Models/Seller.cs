using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthnoBot.Models
{
    [Bind(Exclude = "SellerId")]
    [Table("Sellers")]
    public class Seller
    {
        [Key]
        [ScaffoldColumn(false)]
        public string SellerId { get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }


        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        

        [DisplayName("Seller Description")]
        [Required(ErrorMessage = "Seller Description is required.")]
        public string Description { get; set; }
        [DisplayName("About Seller")]
        public string About { get; set; }
        [DisplayName("Address Line 1")]
        [Required(ErrorMessage = "Address Line 1 is required.")]
        public string AddressLine1 { get; set; }
        [DisplayName("Address Line 2")]
        [Required(ErrorMessage = "Address Line 2 is required.")]
        public string AddressLine2 { get; set; }
        [DisplayName("Address Line 3")]
        [Required(ErrorMessage = "Address Line 3 is required.")]
        public string AddressLine3 { get; set; }

        [DisplayName("PostCode")]
        [Required(ErrorMessage = "PostCode is required.")]
        public string PostCode { get; set; }

        [DisplayName("City")]
        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [DisplayName("Country")]
        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }

        [DisplayName("Quality of Service Rating")]
        public int AverageRating { get; set; }
        [DisplayName("Mobile Number")]
        public string Mobile { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }

        public string ASPUserId{ get; set; }
        public bool IsVerified { get; set; }
       
    }
}