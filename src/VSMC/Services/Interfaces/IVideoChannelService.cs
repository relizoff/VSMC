using System;
using System.Threading;
using System.Threading.Tasks;
using VSMC.Models;

namespace VSMC.Services.Interfaces
{
    public interface IVideoChannelService
    {
        Task LoadAsync();

        Task SearchAsync(string query, Action<VideoModel> videoFound, Action<ChannelModel> channelFound, CancellationToken cancellationToken);
        Task<string> GetStreamUrlAsync(VideoModel video);
        Task<ChannelModel> GetLocalChannelAsync(ChannelModel channel, CancellationToken cancellationToken);
        Task RefreshChannelAsync(ChannelModel channel, CancellationToken cancellationToken);
        Task<VideoModel> GetLocalVideoAsync(VideoModel video, CancellationToken cancellationToken);
    }
}