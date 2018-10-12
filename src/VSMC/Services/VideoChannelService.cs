using Catel.Fody;
using Catel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catel.Collections;
using VSMC.Models;
using VSMC.Services.Interfaces;

namespace VSMC.Services
{
    public class VideoChannelService : IVideoChannelService
    {
        private readonly ApplicationModel _applicationModel;
        private readonly IPleaseWaitService _pleaseWaitService;
        private readonly IEnumerable<IVideoProvider> _videoProviders;
        private readonly INavigationService _navigationService;
        private readonly IStorageService _storageService;
        private TimeSpan _refreshPeriod = TimeSpan.FromHours(1);
        private bool _isLoaded;

        private CancellationTokenSource _cancellationTokenSource;

        public VideoChannelService(
            [NotNull]ApplicationModel applicationModel,
            [NotNull]IPleaseWaitService pleaseWaitService,
            [NotNull]IEnumerable<IVideoProvider> videoProviders,
            [NotNull]INavigationService navigationService,
            [NotNull]IStorageService storageService)
        {
            _applicationModel = applicationModel;
            _pleaseWaitService = pleaseWaitService;
            _videoProviders = videoProviders;
            _navigationService = navigationService;
            _storageService = storageService;
        }

        public async Task LoadAsync()
        {
            if (_isLoaded) return;

            _pleaseWaitService.Show();
            try
            {
                using (_applicationModel.Channels.SuspendChangeNotifications())
                {
                    _applicationModel.Channels.ReplaceRange(await _storageService.LoadChannelsAsync());
                }

                _isLoaded = true;
            }
            finally
            {
                _pleaseWaitService.Hide();
            }
        }

        public async Task SearchAsync(string query, Action<VideoModel> videoFound, Action<ChannelModel> channelFound, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return;
            }

            foreach (var videoProvider in _videoProviders)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var videos = await videoProvider.SearchAsync(query, videoFound, channelFound, cancellationToken);

                //TODO: Cache search responces
            }
        }


        public async Task RefreshChannelAsync(ChannelModel channel, CancellationToken cancellationToken)
        {
            if (channel.Updated.Add(_refreshPeriod) < DateTime.Now)
            {
                var provider = _videoProviders.First(x => x.ProviderType == channel.ProviderType);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await provider.UpdateChannelVideoAsync(channel);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await _storageService.SaveChannelsAsync(new[] { channel }, cancellationToken);
            }
        }

        public async Task<VideoModel> GetLocalVideoAsync(VideoModel video, CancellationToken cancellationToken)
        {
            if (video.Id == null)
            {
                var localChannel = await GetLocalChannelAsync(video.Channel, cancellationToken);
                return localChannel.Videos.FirstOrDefault(x => x.ProviderVideoId == video.ProviderVideoId);
            }

            return video;
        }


        public async Task<ChannelModel> GetLocalChannelAsync(ChannelModel channel, CancellationToken cancellationToken)
        {
            if (channel.Id == null)
            {
                var savedChannel = _applicationModel.Channels.FirstOrDefault(x =>
                    x.ProviderType == channel.ProviderType && x.ProviderChannelId == channel.ProviderChannelId);
                if (savedChannel != null)
                {
                    channel = savedChannel;
                }
                else
                {
                    _applicationModel.Channels.Insert(0, channel);

                    await _storageService.SaveChannelsAsync(new[] { channel }, cancellationToken);
                }
            }

            return channel;
        }

        public async Task<string> GetStreamUrlAsync(VideoModel video)
        {
            _pleaseWaitService.Show();
            try
            {
                var provider = _videoProviders.First(x => x.ProviderType == video.ProviderType);
                var streams = await provider.GetStreamUrlsAsync(video);

                var stream = streams.LastOrDefault(x => x.Key <= StreamVideoQuality.High720);

                return stream.Value;
            }
            finally
            {
                _pleaseWaitService.Hide();
            }
        }
    }
}