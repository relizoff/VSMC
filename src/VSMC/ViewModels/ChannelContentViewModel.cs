using System;
using System.Collections.Specialized;
using Catel.Collections;
using Catel.Fody;
using VSMC.Models;
using VSMC.ViewControllers;
using System.Linq;
using System.Threading;
using Catel.Windows.Threading;

namespace VSMC.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class ChannelContentViewModel : ViewModelBase
    {
        private readonly ApplicationController _applicationController;

        public ChannelContentViewModel([NotNull]ApplicationController applicationController)
        {
            _applicationController = applicationController;

            Videos = new FastObservableCollection<VideoModel>();
        }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        public ChannelModel CurrentChannel { get; set; }

        public FastObservableCollection<VideoModel> Videos { get; set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // TODO: subscribe to events here
        }

        protected override async void OnNavigationCompleted()
        {
            base.OnNavigationCompleted();

            if (NavigationContext.Values.TryGetValue("ProviderChannelId", out var providerChannelId) &&
                NavigationContext.Values.TryGetValue("ProviderType", out var providerType) &&
                Enum.TryParse(providerType as string, false, out ProviderType type))
            {
                CurrentChannel = _applicationController.GetChannel(type, providerChannelId as string);

                var ct = _applicationController.Run(LoadVideos);
                if (ct.IsCancellationRequested) return;

                CurrentChannel.Videos.CollectionChanged += OnVideosCollectionChanged;
                await _applicationController.RefreshChannelAsync(CurrentChannel);
            }
        }

        protected override async Task CloseAsync()
        {
            if (CurrentChannel != null)
            {
                CurrentChannel.Videos.CollectionChanged -= OnVideosCollectionChanged;
            }

            await base.CloseAsync();
        }

        private void OnVideosCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count == 1 && Videos.Count == e.NewStartingIndex)
            {
                Videos.Add((VideoModel)e.NewItems[0]);
                DispatcherHelper.DoEvents();
                return;
            }

            Videos.ReplaceRange(CurrentChannel.Videos);
        }

        private void LoadVideos(CancellationToken ct)
        {
            const int numberOfPreloadVideos = 14;

            var videos = CurrentChannel.Videos.ToArray();
            Videos.Clear();
            Videos.AddItems(videos.Take(numberOfPreloadVideos));
            DispatcherHelper.DoEvents();

            foreach (var video in videos.Skip(numberOfPreloadVideos))
            {
                if (ct.IsCancellationRequested) return;

                Videos.Add(video);
                DispatcherHelper.DoEvents();
            }
        }
    }
}
