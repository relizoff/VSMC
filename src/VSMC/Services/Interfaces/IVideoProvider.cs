using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VSMC.Models;

namespace VSMC.Services.Interfaces
{
    public interface IVideoProvider
    {
        ProviderType ProviderType { get; }

        Task<IEnumerable<VideoModel>> SearchAsync(string query, Action<VideoModel> videoFound, Action<ChannelModel> channelFound, CancellationToken cancellationToken);
        Task UpdateChannelVideoAsync(ChannelModel channel);
        Task<IDictionary<StreamVideoQuality, string>> GetStreamUrlsAsync(VideoModel video);
    }
}