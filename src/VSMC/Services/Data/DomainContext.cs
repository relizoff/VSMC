using System.Data.Entity;

namespace VSMC.Services.Data
{
    public class DomainContext: DbContext
    {
        public DomainContext():base("DefaultConnection")
        {
        }
        
        public DbSet<ChannelDomain> Channels { get; set; }
        
        public DbSet<VideoDomain> Videos { get; set; }
    }
}