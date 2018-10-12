using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catel.Windows.Threading;
using VSMC.Models;
using VSMC.Services.Data;
using VSMC.Services.Interfaces;

namespace VSMC.Services
{
    public class StorageService : IStorageService
    {
        public async Task<IEnumerable<ChannelModel>> LoadChannelsAsync()
        {
            using (var context = new DomainContext())
            {
                await context.Videos.LoadAsync();
                await context.Channels.LoadAsync();

                return context.Channels.Local.Select(DomainToModel).ToArray();
            }
        }

        public async Task SaveChannelsAsync(IEnumerable<ChannelModel> channels, CancellationToken cancellationToken)
        {
            using (var context = new DomainContext())
            {
                foreach (var channel in channels)
                {
                    if (cancellationToken.IsCancellationRequested) return;

                    ChannelDomain channelDomain = null;

                    if (channel.Id != null)
                    {
                        channelDomain = await context.Channels.FirstOrDefaultAsync(x=>x.Id == channel.Id, cancellationToken);
                    }
                    else
                    {
                        channelDomain = await context.Channels.FirstOrDefaultAsync(x=>x.ProviderType == (int)channel.ProviderType && x.ProviderChannelId == channel.ProviderChannelId, cancellationToken);
                        if (channelDomain != null)
                        {
                            channel.Id = channelDomain.Id;
                        }
                    }

                    if (channelDomain == null)
                    {
                        channelDomain = new ChannelDomain()
                        {
                            ProviderType = (int) channel.ProviderType,
                            ProviderChannelId = channel.ProviderChannelId,
                        };
                        context.Channels.Add(channelDomain);
                    }

                    channelDomain.IsPinned = channel.IsPinned;
                    channelDomain.Title = channel.Title;
                    channelDomain.LogoUrl = channel.LogoUrl;
                    channelDomain.Updated = channel.Updated.ToBinary();

                    if (context.Entry(channelDomain).State != EntityState.Unchanged)
                    {
                        channel.Saved = DateTime.Now;
                        channelDomain.Saved = channel.Saved.ToBinary();
                        
                        //await context.SaveChangesAsync(cancellationToken);
                        //channel.Id = channelDomain.Id;
                    }

                    foreach (var video in channel.Videos)
                    {
                        if (cancellationToken.IsCancellationRequested) return;

                        VideoDomain videoDomain = null;

                        if (video.Id != null)
                        {
                            videoDomain = await context.Videos.FirstOrDefaultAsync(x => x.Id == video.Id, cancellationToken);
                        }
                        else if (video.Channel?.Id != null)
                        {
                            videoDomain = await context.Videos.FirstOrDefaultAsync(x => x.ChannelId == video.Channel.Id && x.ProviderVideoId == video.ProviderVideoId, cancellationToken);
                            if (videoDomain != null)
                            {
                                video.Id = videoDomain.Id;
                            }
                        }

                        if (videoDomain == null)
                        {
                            videoDomain = new VideoDomain()
                            {
                                Channel = channelDomain,
                                ProviderVideoId = video.ProviderVideoId,
                            };

                            context.Videos.Add(videoDomain);
                        }

                        videoDomain.Title = video.Title;
                        videoDomain.Description = video.Description;
                        videoDomain.Author = video.Author;
                        videoDomain.Url = video.Url;
                        videoDomain.ThumbnailUrl = video.ThumbnailUrl;
                        videoDomain.Duration = video.Duration.TotalMilliseconds;
                        videoDomain.ViewPosition = video.ViewPosition;
                        videoDomain.Uploaded = video.Uploaded.ToBinary();
                        videoDomain.Viewed = video.Viewed.ToBinary();

                        if (context.Entry(videoDomain).State != EntityState.Unchanged)
                        {
                            video.Saved = DateTime.Now;
                            videoDomain.Saved = video.Saved.ToBinary();
                        
                            //await context.SaveChangesAsync(cancellationToken);
                            //video.Id = videoDomain.Id;
                        }

                        DispatcherHelper.DoEvents();

                    }

                }

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private ChannelModel DomainToModel(ChannelDomain channel)
        {
            var channelModel = new ChannelModel()
            {
                Id = channel.Id,
                ProviderType = (ProviderType) channel.ProviderType,
                Title = channel.Title,
                ProviderChannelId = channel.ProviderChannelId,
                IsPinned = channel.IsPinned,
                LogoUrl = channel.LogoUrl,
                Saved = DateTime.FromBinary(channel.Saved),
                Updated = DateTime.FromBinary(channel.Updated)
            };

            foreach (var video in channel.Videos)
            {
                var videoModel = DomainToModel(video);
                channelModel.Videos.Add(videoModel);
                
                videoModel.Channel = channelModel;
                videoModel.ProviderType = channelModel.ProviderType;
            }

            return channelModel;
        }

        private VideoModel DomainToModel(VideoDomain video)
        {
            var videoModel = new VideoModel()
            {
                Id = video.Id,
                ProviderVideoId = video.ProviderVideoId,
                Title = video.Title,
                Description = video.Description,
                Author = video.Author,
                Url = video.Url,
                ThumbnailUrl = video.ThumbnailUrl,
                ViewPosition = video.ViewPosition,
                Duration = TimeSpan.FromMilliseconds(video.Duration),
                Uploaded = DateTime.FromBinary(video.Uploaded),
                Viewed = DateTime.FromBinary(video.Viewed),
                Saved = DateTime.FromBinary(video.Saved),
            };

            return videoModel;
        }
    }
}