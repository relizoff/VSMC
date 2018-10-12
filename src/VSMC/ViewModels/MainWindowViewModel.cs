using Catel.Data;
using Catel.Fody;
using Catel.IoC;
using Catel.Messaging;
using Catel.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Catel.Collections;
using VSMC.Messages;
using VSMC.Models;
using VSMC.Services.Interfaces;
using VSMC.ViewControllers;

namespace VSMC.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ApplicationController _applicationController;
        private readonly IVideoChannelService _videoChannelService;
        private readonly INavigationService _navigationService;

        public MainWindowViewModel(
            [NotNull]ApplicationModel applicationModel,
            [NotNull]ApplicationController applicationController,
            [NotNull]IVideoChannelService videoChannelService,
            [NotNull]INavigationService navigationService)
        {
            _applicationController = applicationController;
            _videoChannelService = videoChannelService;
            _navigationService = navigationService;

            Application = applicationModel;
            IsTopBarVisible = true;
        }

        public override string Title => "VSMC";

        [Model(SupportIEditableObject = false, SupportValidation = false)]
        public ApplicationModel Application { get; set; }

        public bool IsTopBarVisible { get; set; }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            MessageMediatorHelper.SubscribeRecipient(this);

            await _videoChannelService.LoadAsync();

            NavigateDashboardCommand = new Command(() => { _navigationService.Navigate<DashboardViewModel>(); });
            NavigateChannelsListCommand = new Command(() => { _navigationService.Navigate<ChannelsListViewModel>(); });
            NavigateHistoryCommand = new Command(() => { _navigationService.Navigate<HistoryViewModel>(); });

            _navigationService.Navigate<DashboardViewModel>();
        }

        public Command NavigateDashboardCommand { get; private set; }
        public Command NavigateChannelsListCommand { get; private set; }
        public Command NavigateHistoryCommand { get; private set; }

        protected override async Task CloseAsync()
        {
            MessageMediatorHelper.UnsubscribeRecipient(this);

            await base.CloseAsync();
        }

        public string SearchQuery { get; set; }

        private void OnSearchQueryChanged()
        {
            // this method is automatically called when the SearchQuery property changes
        }

        [MessageRecipient]
        private void OnVideoPlayerOpenedMessage(VideoPlayerOpenedMessage m)
        {
        }

        [MessageRecipient]
        private void OnVideoPlayerClosedMessage(VideoPlayerClosedMessage m)
        {
        }

        #region SearchVideoOrChannel command

        private static Command _searchVideoOrChannelCommand;

        /// <summary>
        /// Gets the SearchVideoOrChannel command.
        /// </summary>
        public Command SearchVideoOrChannelCommand
        {
            get { return _searchVideoOrChannelCommand ?? (_searchVideoOrChannelCommand = new Command(SearchVideoOrChannel)); }
        }

        /// <summary>
        /// Method to invoke when the SearchVideoOrChannel command is executed.
        /// </summary>
        private void SearchVideoOrChannel()
        {
            _applicationController.NavigateToSearch(SearchQuery);
        }

        #endregion

    }
}
