using System.ComponentModel.DataAnnotations;

namespace EthnoBot.Models
{
    public class ListingTagAssociation
    {
        [Key]
        public string ListingTagAssociationId { get; set; }
        public string ListingTagId { get; set; }
        public string ListingId { get; set; }
        
        
    }
}