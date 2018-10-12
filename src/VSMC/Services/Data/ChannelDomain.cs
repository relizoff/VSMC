using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace VSMC.Services.Data
{
    [Table("Channels")]
    public class ChannelDomain
    {
        public int Id { get; set; }

        public int ProviderType { get; set; }

        public string ProviderChannelId { get; set; }

        public string Title { get; set; }

        public string LogoUrl { get; set; }

        public ICollection<VideoDomain> Videos { get; set; }

        public bool IsPinned { get; set; }

        public long Saved { get; set; }
        
        public long Updated { get; set; }

    }
}