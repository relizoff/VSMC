using System.Linq;
using Catel.Collections;
using VSMC.Models;

namespace VSMC.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class ChannelsListViewModel : ViewModelBase
    {
        public ChannelsListViewModel(ApplicationModel applicationModel)
        {
            Application = applicationModel;
            Channels = new FastObservableCollection<ChannelModel>();
        }

        public override string Title => "Channels";

        [Model]
        public ApplicationModel Application { get; set; }

        public FastObservableCollection<ChannelModel> Channels { get; set; }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            UpdateChannels();
        }

        protected override async Task CloseAsync()
        {
            // TODO: unsubscribe from events here

            await base.CloseAsync();
        }

        private void UpdateChannels()
        {
            Channels.AddItems(Application.Channels.Where(x=>x.IsPinned).OrderBy(x=>x.Title));
        }

    }
}
