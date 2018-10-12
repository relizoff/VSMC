using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VSMC.Models;

namespace VSMC.Services.Interfaces
{
    public interface IStorageService
    {
        Task<IEnumerable<ChannelModel>> LoadChannelsAsync();
        Task SaveChannelsAsync(IEnumerable<ChannelModel> channels, CancellationToken cancellationToken);
    }
}