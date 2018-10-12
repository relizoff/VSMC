using System.Threading.Tasks;
using Vlc.DotNet.Wpf;
using VSMC.Models;

namespace VSMC.Services.Interfaces
{
    public interface IVideoPlayerService
    {
        Task PlayVideoAsync(VideoModel video);
        void RegisterPlayer(VlcVideoSourceProvider sourceProvider);
        void Stop();
        void ChangePosition(double position);
        void Pause();
        void Resume();
        void UnregisterPlayer(VlcVideoSourceProvider sourceProvider);
    }
}