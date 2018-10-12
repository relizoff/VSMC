using Catel.Data;
using Catel.Fody;
using VSMC.Models;
using VSMC.Services.Interfaces;
using VSMC.ViewControllers;

namespace VSMC.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class ChannelViewModel : ViewModelBase
    {
        private readonly ApplicationController _applicationController;

        public ChannelViewModel(
            [NotNull]ChannelModel channelModel,
            [NotNull]ApplicationController applicationController)
        {
            _applicationController = applicationController;
            Channel = channelModel;
        }

        [Model]
        public ChannelModel Channel { get; set; }

        public override string Title => Channel.Title;

        #region IsPinned property

        /// <summary>
        /// Gets or sets the IsPinned value.
        /// </summary>
        [ViewModelToModel("Channel")]
        public bool IsPinned
        {
            get { return GetValue<bool>(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, value); }
        }

        /// <summary>
        /// IsPinned property data.
        /// </summary>
        public static readonly PropertyData IsPinnedProperty =
            RegisterProperty("IsPinned", typeof(bool), null, (sender, e) => ((ChannelViewModel) sender).OnIsPinnedChanged());

        /// <summary>
        /// Called when the IsPinned property has changed.
        /// </summary>
        private async Task OnIsPinnedChanged()
        {
            if (IsPinned)
            {
                await _applicationController.PinChannelAsync(Channel);
            }
            else
            {
                await _applicationController.UnpinChannelAsync(Channel);
            }
        }

        #endregion

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
        private  void OpenChannel()
        {
            _applicationController.NavigateToChannel(Channel);
        }

        #endregion

        #region OpenChannelInBrowser command

        private Command _openChannelInBrowserCommand;

        /// <summary>
        /// Gets the OpenChannelInBrowser command.
        /// </summary>
        public Command OpenChannelInBrowserCommand
        {
            get { return _openChannelInBrowserCommand ?? (_openChannelInBrowserCommand = new Command(OpenChannelInBrowser)); }
        }

        /// <summary>
        /// Method to invoke when the OpenChannelInBrowser command is executed.
        /// </summary>
        private void OpenChannelInBrowser()
        {
            // TODO: Handle command logic here
        }

        #endregion
    }
}
