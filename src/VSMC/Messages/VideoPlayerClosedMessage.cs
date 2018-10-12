using Catel.Messaging;
using VSMC.Models;

namespace VSMC.Messages
{
    public class VideoPlayerClosedMessage : MessageBase<VideoPlayerClosedMessage, VideoModel>
    {
        public VideoPlayerClosedMessage()
        {
        }

        public VideoPlayerClosedMessage(VideoModel data) : base(data)
        {
        }
    }
}