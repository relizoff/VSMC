using Catel.Collections;
using Catel.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VSMC.Models
{
    [Serializable]
    public class ApplicationModel : DispatcherObservableObject 
    {
        public ApplicationModel()
        {
            Channels = new FastObservableCollection<ChannelModel> { AutomaticallyDispatchChangeNotifications = true };
        }

        public ChannelModel CurrentChannel { get; set; }

        public VideoModel CurrentVideo { get; set; }

        public FastObservableCollection<ChannelModel> Channels { get; set; }

        public double CurrentPosition { get; set; }
    }
}
