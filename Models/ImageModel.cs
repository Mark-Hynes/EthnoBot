using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class ImageModel
    {
        [Key]
        public int id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}