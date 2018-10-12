using Catel.Messaging;
using VSMC.Models;

namespace VSMC.Messages
{
    public class VideoPlayerOpenedMessage : MessageBase<VideoPlayerOpenedMessage, VideoModel>
    {
        public VideoPlayerOpenedMessage()
        {
        }

        public VideoPlayerOpenedMessage(VideoModel data) : base(data)
        {
        }
    }
}