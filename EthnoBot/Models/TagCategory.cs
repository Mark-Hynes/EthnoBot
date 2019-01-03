using System.ComponentModel.DataAnnotations;

namespace EthnoBot.Models
{
    public class TagCategory
    {
        [Key]
        public string TagCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}