using Catel.Messaging;
using VSMC.Models;

namespace VSMC.Messages
{
    public class VideoPlayerPauseMessage : MessageBase<VideoPlayerPauseMessage, VideoModel>
    {
        public VideoPlayerPauseMessage()
        {
        }

        public VideoPlayerPauseMessage(VideoModel data) : base(data)
        {
        }
    }
}