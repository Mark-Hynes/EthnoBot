using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace EthnoBot.Models
{
    public class RDSContext : DbContext
    {
    

        public static RDSContext Create()
        {
            return new RDSContext();
        }
    }
}