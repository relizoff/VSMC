using Catel.Collections;
using Catel.Fody;
using Catel.Services;
using System.Collections.Generic;
using Catel.IoC;
using VSMC.Models;
using VSMC.Services.Interfaces;
using VSMC.ViewControllers;

namespace VSMC.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class SearchViewModel : NavigationViewModelBase
    {
        private readonly ApplicationController _applicationController;

        public SearchViewModel(
            [NotNull]ApplicationModel applicationModel,
            [NotNull]ApplicationController applicationController)
        {
            _applicationController = applicationController;
            Application = applicationModel;

            FoundChannels = new FastObservableCollection<ChannelModel> { AutomaticallyDispatchChangeNotifications = true };
            FoundVideos = new FastObservableCollection<VideoModel> { AutomaticallyDispatchChangeNotifications = true };
        }

        [Model]
        public ApplicationModel Application { get; set; }

        public FastObservableCollection<ChannelModel> FoundChannels { get; set; }

        public FastObservableCollection<VideoModel> FoundVideos { get; set; }

        public override string Title => "Search";

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        protected override async void OnNavigationCompleted()
        {
            base.OnNavigationCompleted();
            if (NavigationContext.Values.TryGetValue("query", out var query))
            {
                await SearchAsync(query as string);
            }
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // TODO: subscribe to events here
        }

        protected override async Task CloseAsync()
        {
            // TODO: unsubscribe from events here

            await base.CloseAsync();
        }

        public async Task SearchAsync(string query)
        {
            FoundVideos.Clear();
            FoundChannels.Clear();

            await _applicationController.SearchAsync(
                query,
                video => FoundVideos.Add(video),
                channel => FoundChannels.Add(channel));
        }
    }
}
