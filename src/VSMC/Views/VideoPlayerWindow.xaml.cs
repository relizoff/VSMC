using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Catel.IoC;
using VSMC.Services.Interfaces;

namespace VSMC.Views
{
    public partial class VideoPlayerWindow
    {
        private DateTime _lastMouseActivity;
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer(DispatcherPriority.Input);
        private readonly Storyboard _showControlsStoryboard;
        private readonly Storyboard _hideControlsStoryboard;
        private readonly IVideoPlayerService _videoPlayerService;

        public VideoPlayerWindow()
        {
            InitializeComponent();

            var dependencyResolver = this.GetDependencyResolver();
            _videoPlayerService = dependencyResolver.Resolve<IVideoPlayerService>();
            _videoPlayerService.RegisterPlayer(VideoPlayerControl.SourceProvider);

            _showControlsStoryboard = Resources["ShowControlsStoryboard"] as Storyboard;
            _hideControlsStoryboard = Resources["HideControlsStoryboard"] as Storyboard;

            Debug.Assert(_showControlsStoryboard != null, nameof(_showControlsStoryboard) + " != null");
            _showControlsStoryboard.Begin(this, HandoffBehavior.SnapshotAndReplace, true);
            _showControlsStoryboard.Stop(this);

            Debug.Assert(_hideControlsStoryboard != null, nameof(_hideControlsStoryboard) + " != null");
            _hideControlsStoryboard.Begin(this, HandoffBehavior.SnapshotAndReplace, true);
            _hideControlsStoryboard.Stop(this);


            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Tick += OnDispatcherTimerTick;

            _dispatcherTimer.Start();

            _lastMouseActivity = DateTime.Now;
        }

        protected override void OnUnloaded(EventArgs e)
        {
            _videoPlayerService.UnregisterPlayer(VideoPlayerControl.SourceProvider);

            base.OnUnloaded(e);
        }

        private void OnDispatcherTimerTick(object sender, EventArgs e)
        {
            if (_lastMouseActivity.AddSeconds(3) < DateTime.Now)
            {
                if (_showControlsStoryboard.GetCurrentState(this) != ClockState.Stopped)
                {
                    _showControlsStoryboard.Stop(this);
                }

                if (_hideControlsStoryboard.GetCurrentState(this) == ClockState.Stopped)
                {
                    BeginStoryboard(_hideControlsStoryboard, HandoffBehavior.SnapshotAndReplace, true);
                }

                _lastMouseActivity = DateTime.Now;
            }
        }


        private void OnMouseActivity(object sender, MouseEventArgs e)
        {
            if (_hideControlsStoryboard.GetCurrentState(this) != ClockState.Stopped)
            {
                _hideControlsStoryboard.Stop(this);
            }

            if (_showControlsStoryboard.GetCurrentState(this) == ClockState.Stopped)
            {
                BeginStoryboard(_showControlsStoryboard, HandoffBehavior.SnapshotAndReplace, true);
            }

            _lastMouseActivity = DateTime.Now;
        }
    }
}
