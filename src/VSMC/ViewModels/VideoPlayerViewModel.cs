using System;
using System.ComponentModel;
using Catel.Data;
using Catel.Fody;
using Catel.Messaging;
using Catel.Services;
using Catel.Windows.Threading;
using VSMC.Messages;
using VSMC.Models;
using VSMC.Services.Interfaces;

namespace VSMC.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class VideoPlayerViewModel : NavigationViewModelBase
    {
        private readonly IVideoPlayerService _videoPlayerService;
        private readonly INavigationService _navigationService;

        public VideoPlayerViewModel(
            [NotNull]ApplicationModel applicationModel,
            [NotNull]IVideoPlayerService videoPlayerService,
            [NotNull]INavigationService navigationService)
        {
            _videoPlayerService = videoPlayerService;
            _navigationService = navigationService;
            Application = applicationModel;

            ThrottlingRate = TimeSpan.FromSeconds(0.5);
        }

        [Model(SupportIEditableObject = false, SupportValidation = false)]
        public ApplicationModel Application { get; set; }

        [ViewModelToModel("Application")]
        public VideoModel CurrentVideo { get; set; }

        [ViewModelToModel("Application")]
        public double CurrentPosition { get; set; }

        public bool IsPaused { get; set; }

        public double EditableCurrentPosition
        {
            get => CurrentPosition;
            set => SetCurrentPosition(value);
        }

        private void SetCurrentPosition(double position)
        {
            _videoPlayerService.ChangePosition(position);
            CurrentPosition = position;
        }

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentPosition")
            {
                OnPropertyChanged(new AdvancedPropertyChangedEventArgs(this, "EditableCurrentPosition"));

                CurrentVideo.ViewPosition = CurrentPosition;
            }
            base.OnPropertyChanged(e);
        }


        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            MessageMediatorHelper.SubscribeRecipient(this);

            VideoPlayerOpenedMessage.SendWith(CurrentVideo);

            await _videoPlayerService.PlayVideoAsync(CurrentVideo);
        }

        protected override async Task CloseAsync()
        {
            VideoPlayerClosedMessage.SendWith(CurrentVideo);

            MessageMediatorHelper.UnsubscribeRecipient(this);

            await Task.Run(() => _videoPlayerService.Stop());

            await base.CloseAsync();
        }

        [MessageRecipient]
        private void OnVideoPlayerPositionMessage(VideoPlayerPositionMessage m)
        {
            CurrentPosition = m.Data;
        }

        [MessageRecipient]
        private void OnVideoPlayerStopMessage(VideoPlayerStopMessage m)
        {
            DispatcherHelper.CurrentDispatcher.BeginInvoke(async () => await CloseViewModelAsync(null));
        }

        [MessageRecipient]
        private void OnVideoPlayerPauseMessage(VideoPlayerPauseMessage m)
        {
            IsPaused = true;

            StopPlayerCommand.RaiseCanExecuteChanged();
            PausePlayerCommand.RaiseCanExecuteChanged();
            ResumePlayerCommand.RaiseCanExecuteChanged();
        }

        [MessageRecipient]
        private void OnVideoPlayerPlayMessage(VideoPlayerPlayMessage m)
        {
            IsPaused = false;

            StopPlayerCommand.RaiseCanExecuteChanged();
            PausePlayerCommand.RaiseCanExecuteChanged();
            ResumePlayerCommand.RaiseCanExecuteChanged();
        }

        #region StopPlayer command

        private TaskCommand _stopPlayerCommand;

        /// <summary>
        /// Gets the StopPlayer command.
        /// </summary>
        public TaskCommand StopPlayerCommand => _stopPlayerCommand ?? (_stopPlayerCommand = new TaskCommand(() => Task.Run(() => _videoPlayerService.Stop())));

        #endregion

        #region PausePlayer command

        private TaskCommand _pausePlayerCommand;

        /// <summary>
        /// Gets the PausePlayer command.
        /// </summary>
        public TaskCommand PausePlayerCommand
        {
            get { return _pausePlayerCommand ?? (_pausePlayerCommand = new TaskCommand(() => Task.Run(() => _videoPlayerService.Pause()), () => !IsPaused )); }
        }

        #endregion

        #region ResumePlayer command

        private TaskCommand _resumePlayerCommand;

        /// <summary>
        /// Gets the ResumePlayer command.
        /// </summary>
        public TaskCommand ResumePlayerCommand
        {
            get { return _resumePlayerCommand ?? (_resumePlayerCommand = new TaskCommand(() => Task.Run(() => _videoPlayerService.Resume()), () => IsPaused )); }
        }

        #endregion
    }
}
