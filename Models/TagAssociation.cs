using System.ComponentModel.DataAnnotations;

namespace EthnoBot.Models
{
    public class TagAssociation
    {
        [Key]
        public string TagAssociationId { get; set; }
        public string TagId { get; set; }
        public string ProductId { get; set; }
        
        
    }
}