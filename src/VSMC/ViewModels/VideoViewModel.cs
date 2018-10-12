using System;
using Catel.Data;
using Catel.Fody;
using VSMC.Models;
using VSMC.Services.Interfaces;
using VSMC.ViewControllers;

namespace VSMC.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class VideoViewModel : ViewModelBase
    {
        private readonly ApplicationController _applicationController;

        public VideoViewModel(
            [NotNull]VideoModel videoModel,
            [NotNull]ApplicationController applicationController)
        {
            _applicationController = applicationController;
            Video = videoModel;
        }

        [Model]
        public VideoModel Video { get; set; }

        [ViewModelToModel("Video")]
        public ChannelModel Channel { get; set; }

        public bool IsViewEnded
        {
            get => Video.IsViewEnded;
            set
            {
                Video.ViewPosition = value ? 1.0 : 0;
                OnPropertyChanged(new AdvancedPropertyChangedEventArgs(this, "IsViewEnded"));
            }
        }

        public override string Title => Video.Title;

        public string Duration
        {
            get
            {
                if (Video.Duration.TotalHours >= 1)
                {
                    return $"{Video.Duration.Hours}:{Video.Duration.Minutes:00}:{Video.Duration.Seconds:00}";
                }
                return $"{Video.Duration.Minutes}:{Video.Duration.Seconds:00}";
            }
        }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

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

        #region OpenChannel command

        private Command _openChannelCommand;

        /// <summary>
        /// Gets the OpenChannel command.
        /// </summary>
        public Command OpenChannelCommand
        {
            get { return _openChannelCommand ?? (_openChannelCommand = new Command(OpenChannel)); }
        }

        /// <summary>
        /// Method to invoke when the OpenChannel command is executed.
        /// </summary>
        private void OpenChannel()
        {
            _applicationController.NavigateToChannel(Channel);
        }

        #endregion

        #region PlayVideo command

        private Command _playVideoCommand;

        /// <summary>
        /// Gets the PlayVideo command.
        /// </summary>
        public Command PlayVideoCommand
        {
            get { return _playVideoCommand ?? (_playVideoCommand = new Command(PlayVideo)); }
        }

        /// <summary>
        /// Method to invoke when the PlayVideo command is executed.
        /// </summary>
        private void PlayVideo()
        {
            _applicationController.OpenVideoPlayer(Video);
        }

        #endregion
    }
}
