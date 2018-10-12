using Catel.Messaging;

namespace VSMC.Messages
{
    public class VideoPlayerPositionMessage : MessageBase<VideoPlayerPositionMessage, float>
    {
        public VideoPlayerPositionMessage()
        {
        }

        public VideoPlayerPositionMessage(float data) : base(data)
        {
        }
    }
}