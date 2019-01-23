using System.ComponentModel.DataAnnotations;

namespace EthnoBot.Models
{
    public class ListingTagCategory
    {
        [Key]
        public string ListingTagCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ListingTagType { get; set; }
    }
}