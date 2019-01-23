using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EthnoBot.Models
{
    public class ListingTag
    {
        [Key]
        public string ListingTagId { get; set; }
        [Column("ListingTagType")]
        public string ListingTagType { get; set; }
        [Column("ListingTagCategoryId")]
        public string ListingTagCategoryId { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}