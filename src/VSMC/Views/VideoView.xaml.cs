using System.Windows;

namespace VSMC.Views
{
    public partial class VideoView
    {
        public VideoView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ChannelButtonVisibilityProperty = DependencyProperty.Register(
            "ChannelButtonVisibility", typeof(Visibility), typeof(VideoView), new PropertyMetadata(Visibility.Visible));

        public Visibility ChannelButtonVisibility
        {
            get { return (Visibility) GetValue(ChannelButtonVisibilityProperty); }
            set { SetValue(ChannelButtonVisibilityProperty, value); }
        }
    }
}
