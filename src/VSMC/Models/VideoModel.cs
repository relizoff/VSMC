using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;
using VSMC.Services.Interfaces;

namespace VSMC.Models
{
    public class VideoModel : DispatcherObservableObject 
    {
        public VideoModel()
        {
        }

        public int? Id { get; set; }

        public ProviderType ProviderType { get; set; }

        public string ProviderVideoId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string Url { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime Uploaded { get; set; }

        public DateTime Saved { get; set; }
        
        public DateTime Viewed { get; set; }
        
        public bool IsViewEnded => ViewPosition > 0.98;

        public double ViewPosition { get; set; }

        public string ThumbnailUrl { get; set; }

        public ChannelModel Channel { get; set; }

    }
}
