using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EthnoBot.Models
{
    public class Tag
    {
        [Key]
        public string TagId { get; set; }
        [Column("TagType")]
        public string TagType { get; set; }
        [Column("TagCategoryId")]
        public string TagCategoryId { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}