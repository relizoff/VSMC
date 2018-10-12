using System;
using System.Collections.Generic;
using System.Linq;
using Catel.Fody;
using Catel.Services;
using System.Threading;
using System.Threading.Tasks;
using Catel.IoC;
using Catel.MVVM;
using Catel.MVVM.Views;
using VSMC.Models;
using VSMC.Services.Interfaces;
using VSMC.ViewModels;
using VSMC.Views;

namespace VSMC.ViewControllers
{
    public class ApplicationController : CancelableControllerBase
    {
        private readonly ApplicationModel _applicationModel;
        private readonly INavigationService _navigationService;
        private readonly IPleaseWaitService _pleaseWaitService;
        private readonly IVideoChannelService _videoChannelService;
        private readonly IVideoPlayerService _videoPlayerService;
        private readonly IViewModelManager _viewModelManager;
        private readonly IUIVisualizerService _uiVisualizerService;

        public ApplicationController(
            ApplicationModel applicationModel,
            INavigationService navigationService,
            IPleaseWaitService pleaseWaitService,
            IVideoChannelService videoChannelService,
            IVideoPlayerService videoPlayerService,
            IViewModelManager viewModelManager,
            IUIVisualizerService uiVisualizerService)
        {
            _applicationModel = applicationModel;
            _navigationService = navigationService;
            _pleaseWaitService = pleaseWaitService;
            _videoChannelService = videoChannelService;
            _videoPlayerService = videoPlayerService;
            _viewModelManager = viewModelManager;
            _uiVisualizerService = uiVisualizerService;

            _uiVisualizerService.Register<VideoPlayerViewModel,VideoPlayerWindow>();
        }

        #region Overrides

        protected override void OnStartOperation()
        {
            _pleaseWaitService.Push();
            base.OnStartOperation();
        }

        protected override void OnEndOperation(CancellationToken cancellationToken)
        {
            base.OnEndOperation(cancellationToken);
            _pleaseWaitService.Pop();
        }



        #endregion

        #region Navigation

        public void NavigateToSearch(string query)
        {
            var parameters = new Dictionary<string, object> { { "query", query } };
            _navigationService.Navigate<SearchViewModel>(parameters);
        }

        public void NavigateToChannel(ChannelModel channel)
        {
            Run(async ct =>
            {
                var localChannel = await _videoChannelService.GetLocalChannelAsync(channel, ct);

                var parameters = new Dictionary<string, object>
                {
                    {"ProviderChannelId", localChannel.ProviderChannelId},
                    {"ProviderType", localChannel.ProviderType.ToString()}
                };

                _navigationService.Navigate<ChannelContentViewModel>(parameters);

                await _videoChannelService.RefreshChannelAsync(localChannel, ct);
            });
        }

        public void OpenVideoPlayer(VideoModel video)
        {
            Run(async ct =>
            {
                _applicationModel.CurrentVideo = await _videoChannelService.GetLocalVideoAsync(video, ct);
                _applicationModel.CurrentChannel = _applicationModel.CurrentVideo.Channel;

                var videoPlayerWindow =
                    _viewModelManager.ActiveViewModels.OfType<VideoPlayerViewModel>().FirstOrDefault();

                if (videoPlayerWindow == null)
                {
                    await _uiVisualizerService.ShowAsync<VideoPlayerViewModel>();
                }
                else
                {
                    await _videoPlayerService.PlayVideoAsync(_applicationModel.CurrentVideo);
                }
            });
        }

        #endregion

        public async Task RefreshChannelAsync([NotNull] ChannelModel channel)
        {
            await RunAsync(async ct =>
            {
                _applicationModel.CurrentChannel = await _videoChannelService.GetLocalChannelAsync(channel, ct);
                if (ct.IsCancellationRequested) return;

                await _videoChannelService.RefreshChannelAsync(channel, ct);
            });
        }

        public async Task SearchAsync(string query, Action<VideoModel> videoFound, Action<ChannelModel> channelFound)
        {
            await RunAsync(ct => _videoChannelService.SearchAsync(query, videoFound, channelFound, ct));

        }

        public ChannelModel GetChannel(ProviderType providerType, string providerChannelId)
        {
            return _applicationModel.Channels.FirstOrDefault(x => 
                x.ProviderType == providerType && 
                x.ProviderChannelId == providerChannelId);
        }

        public async Task PinChannelAsync(ChannelModel channel)
        {
            await RunAsync(async ct =>
            {
                channel = await _videoChannelService.GetLocalChannelAsync(channel, ct);
                channel.IsPinned = true;
                await _videoChannelService.RefreshChannelAsync(channel, ct);
            });
        }

        public async Task UnpinChannelAsync(ChannelModel channel)
        {
            await RunAsync(async ct =>
            {
                channel = await _videoChannelService.GetLocalChannelAsync(channel, ct);
                channel.IsPinned = false;
            });
        }
    }
}
