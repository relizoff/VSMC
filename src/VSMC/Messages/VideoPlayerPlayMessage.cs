using Catel.Messaging;
using VSMC.Models;

namespace VSMC.Messages
{
    public class VideoPlayerPlayMessage : MessageBase<VideoPlayerPlayMessage, VideoModel>
    {
        public VideoPlayerPlayMessage()
        {
        }

        public VideoPlayerPlayMessage(VideoModel data) : base(data)
        {
        }
    }
}