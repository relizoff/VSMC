using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using Catel.Fody;
using Catel.Logging;
using Catel.Services;
using Catel.Threading;
using Catel.Windows.Threading;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;
using VSMC.Messages;
using VSMC.Models;
using VSMC.Services.Interfaces;
using VSMC.ViewModels;

namespace VSMC.Services
{
    public class VideoPlayerService : IVideoPlayerService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ApplicationModel _applicationModel;
        private readonly IVideoChannelService _videoChannelService;
        private VlcVideoSourceProvider _provider;

        public VideoPlayerService(
            [NotNull]ApplicationModel applicationModel,
            [NotNull]IVideoChannelService videoChannelService)
        {
            _applicationModel = applicationModel;
            _videoChannelService = videoChannelService;
        }

        public async Task PlayVideoAsync([NotNull]VideoModel video)
        {
            VideoPlayerPositionMessage.SendWith(0);

            var url = await _videoChannelService.GetStreamUrlAsync(video);

            _provider?.MediaPlayer.Play(new Uri(url));
        }

        public void RegisterPlayer(VlcVideoSourceProvider provider)
        {
            _provider = provider;

            if (_provider != null)
            {
                var vlcLibDirectory = new DirectoryInfo("C:\\Program Files (x86)\\VideoLAN\\VLC");
                _provider.CreatePlayer(vlcLibDirectory);
                _provider.MediaPlayer.Log += OnMediaPlayerLog;
                _provider.MediaPlayer.PositionChanged += OnMediaPlayerPositionChanged;
                _provider.MediaPlayer.EncounteredError += OnMediaPlayerEncounteredError;
                _provider.MediaPlayer.Stopped += OnMediaPlayerStopped;
                _provider.MediaPlayer.Paused += OnMediaPlayerPaused;
                _provider.MediaPlayer.Playing += OnMediaPlayerPlaying;
            }
        }

        public void UnregisterPlayer([NotNull]VlcVideoSourceProvider provider)
        {
            if (_provider != provider) return;

            _provider = null;

            DispatcherHelper.CurrentDispatcher.InvokeAsync(() =>
            {
                try
                {
                    provider.MediaPlayer.Dispose();
                    provider.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Warning(ex);
                }
            });
        }

        private void OnMediaPlayerPlaying(object sender, VlcMediaPlayerPlayingEventArgs e)
        {
            VideoPlayerPlayMessage.SendWith(_applicationModel.CurrentVideo);
        }

        private void OnMediaPlayerPaused(object sender, VlcMediaPlayerPausedEventArgs e)
        {
            VideoPlayerPauseMessage.SendWith(_applicationModel.CurrentVideo);
        }

        private void OnMediaPlayerStopped(object sender, VlcMediaPlayerStoppedEventArgs e)
        {
            VideoPlayerStopMessage.SendWith(_applicationModel.CurrentVideo);
        }

        private void OnMediaPlayerEncounteredError(object sender, VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            Debug.Fail(e.ToString());
        }

        public void Stop()
        {
            if (_provider != null)
            {
                _provider.MediaPlayer.Stop();
            }
        }

        public void ChangePosition(double position)
        {
            if (_provider != null && _provider.MediaPlayer.IsSeekable)
            {
                _provider.MediaPlayer.Position = (float)position;
            }
        }

        public void Pause()
        {
            if (_provider != null && _provider.MediaPlayer.IsPausable() && _provider.MediaPlayer.IsPlaying())
            {
                _provider.MediaPlayer.Pause();
            }
        }

        public void Resume()
        {
            if (_provider != null && _provider.MediaPlayer.IsPausable() && !_provider.MediaPlayer.IsPlaying())
            {
                _provider.MediaPlayer.Play();
            }
        }

        private void OnMediaPlayerPositionChanged(object sender, VlcMediaPlayerPositionChangedEventArgs e)
        {
            VideoPlayerPositionMessage.SendWith(e.NewPosition);
        }

        private static void OnMediaPlayerLog(object sender, VlcMediaPlayerLogEventArgs e)
        {
            Log.WriteWithData("VLC", e.Message, LogEvent.Info);
        }

    }
}