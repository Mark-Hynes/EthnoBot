using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class EthnoBotEntities : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<FeatureSearchItem> FeatureSearchItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ImageModel> Images { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ListingTag> ListingTags { get; set; }
        public DbSet<TagCategory> TagCategories {get;set;}
        public DbSet<ListingTagCategory> ListingTagCategories {get;set;}
        public DbSet<TagAssociation> TagAssociations { get; set; }
        public DbSet<ListingTagAssociation> ListingTagAssociations { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}