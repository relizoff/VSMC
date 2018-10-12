using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Catel.Collections;
using Catel.Data;
using VSMC.Services.Interfaces;

namespace VSMC.Models
{
    public class ChannelModel : DispatcherObservableObject 
    {
        public ChannelModel()
        {
            Videos = new FastObservableCollection<VideoModel>();
        }

        public int? Id { get; set; }

        public ProviderType ProviderType { get; set; }

        public string ProviderChannelId { get; set; }

        public string Title { get; set; }

        public string LogoUrl { get; set; }

        public FastObservableCollection<VideoModel> Videos { get; set; }

        public bool IsPinned { get; set; }

        public DateTime Saved { get; set; }
        
        public DateTime Updated{ get; set; }
    }
}