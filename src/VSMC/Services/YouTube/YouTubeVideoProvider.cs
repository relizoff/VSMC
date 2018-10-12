using Catel.Logging;
using Catel.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VSMC.Models;
using VSMC.Services.Interfaces;
using YoutubeExplode;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;

namespace VSMC.Services.YouTube
{
    public class YouTubeVideoProvider : IVideoProvider
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private const int SearchMaxPages = 1;

        public ProviderType ProviderType => ProviderType.YouTube;

        public async Task<IEnumerable<VideoModel>> SearchAsync(string query, Action<VideoModel> videoFound, Action<ChannelModel> channelFound, CancellationToken cancellationToken)
        {
            var client = new YoutubeClient();
            var searchVideos = await client.SearchVideosAsync(query, SearchMaxPages);

            var channels = new List<ChannelModel>();
            var videos = new List<VideoModel>();

            foreach (var searchVideo in searchVideos)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                var video = new VideoModel()
                {
                    ProviderType = ProviderType,
                    Author = searchVideo.Author,
                    ProviderVideoId = searchVideo.Id,
                    Title = searchVideo.Title,
                    Description = searchVideo.Description,
                    Duration = searchVideo.Duration,
                    Uploaded = searchVideo.UploadDate.LocalDateTime,
                    ThumbnailUrl = searchVideo.Thumbnails.HighResUrl,
                    Url = searchVideo.GetUrl()
                };

                try
                {
                    var searchVideoChannel = await client.GetVideoAuthorChannelAsync(searchVideo.Id);
                    var channel = channels.FirstOrDefault(x => x.ProviderChannelId == searchVideoChannel.Id);

                    if (channel == null)
                    {
                        channel = new ChannelModel()
                        {
                            ProviderChannelId = searchVideoChannel.Id,
                            LogoUrl = searchVideoChannel.LogoUrl,
                            Title = searchVideoChannel.Title,
                            ProviderType = ProviderType,
                        };
                        channels.Add(channel);

                        channelFound?.Invoke(channel);
                    }

                    channel.Videos.Add(video);
                    video.Channel = channel;
                    videos.Add(video);

                    videoFound?.Invoke(video);
                }
                catch (VideoUnavailableException ex)
                {
                    Log.WriteWithData(ex.Message, new LogData { { "VideoId", ex.VideoId }, { "Code", ex.Code }, { "Reason", ex.Reason } }, LogEvent.Warning);
                }
            }

            return videos;
        }

        public async Task UpdateChannelVideoAsync(ChannelModel channel)
        {
            var client = new YoutubeClient();
            var channelVideos = await client.GetChannelUploadsAsync(channel.ProviderChannelId);

            foreach (var channelVideo in channelVideos)
            {
                var video = channel.Videos.FirstOrDefault(x => x.ProviderVideoId == channelVideo.Id);
                if (video != null)
                {
                    video.Title = channelVideo.Title;
                    video.ThumbnailUrl = channelVideo.Thumbnails.HighResUrl;
                    video.Description = channelVideo.Description;
                    video.Duration = channelVideo.Duration;
                }
                else
                {
                    video = new VideoModel()
                    {
                        ProviderType = ProviderType,
                        Author = channelVideo.Author,
                        ProviderVideoId = channelVideo.Id,
                        Title = channelVideo.Title,
                        Description = channelVideo.Description,
                        Duration = channelVideo.Duration,
                        Uploaded = channelVideo.UploadDate.LocalDateTime,
                        ThumbnailUrl = channelVideo.Thumbnails.HighResUrl,
                        Url = channelVideo.GetUrl(),
                        Channel = channel,
                    };

                    channel.Videos.Add(video);
                }
            }

            channel.Updated = DateTime.Now;
        }

        public async Task<IDictionary<StreamVideoQuality, string>> GetStreamUrlsAsync(VideoModel videom)
        {
            var client = new YoutubeClient();
            var video = await client.GetVideoMediaStreamInfosAsync(videom.ProviderVideoId);
            var result = new SortedDictionary<StreamVideoQuality, string>();
            foreach (var streamInfo in video.Muxed.OrderBy(x => x.VideoQuality).ToArray())
            {
                var quality = Convert(streamInfo.VideoQuality);
                result[quality] = streamInfo.Url;
            }

            return result;
        }

        private StreamVideoQuality Convert(VideoQuality videoQuality)
        {
            switch (videoQuality)
            {
                case VideoQuality.Low144:
                    return StreamVideoQuality.Low144;
                case VideoQuality.Low240:
                    return StreamVideoQuality.Low240;
                case VideoQuality.Medium360:
                    return StreamVideoQuality.Medium360;
                case VideoQuality.Medium480:
                    return StreamVideoQuality.Medium480;
                case VideoQuality.High720:
                    return StreamVideoQuality.High720;
                case VideoQuality.High1080:
                    return StreamVideoQuality.High1080;
                case VideoQuality.High1440:
                    return StreamVideoQuality.High1440;
                case VideoQuality.High2160:
                    return StreamVideoQuality.High2160;
                case VideoQuality.High2880:
                    return StreamVideoQuality.High2880;
                case VideoQuality.High3072:
                    return StreamVideoQuality.High3072;
                case VideoQuality.High4320:
                    return StreamVideoQuality.High4320;
                default:
                    throw new ArgumentOutOfRangeException(nameof(videoQuality), videoQuality, null);
            }
        }

    }
}