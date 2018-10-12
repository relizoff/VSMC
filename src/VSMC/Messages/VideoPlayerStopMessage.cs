using Catel.Messaging;
using VSMC.Models;

namespace VSMC.Messages
{
    public class VideoPlayerStopMessage : MessageBase<VideoPlayerStopMessage, VideoModel>
    {
        public VideoPlayerStopMessage()
        {
        }

        public VideoPlayerStopMessage(VideoModel data) : base(data)
        {
        }
    }
}