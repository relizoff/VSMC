using System;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Squirrel;
using VSMC.Models;
using VSMC.Services;
using VSMC.Services.Interfaces;
using VSMC.Services.YouTube;
using VSMC.ViewControllers;
using VSMC.ViewModels;
using VSMC.Views;

namespace VSMC
{
    using System.Windows;

    using Catel.ApiCop;
    using Catel.ApiCop.Listeners;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            LogManager.AddDebugListener();
#endif

            Log.Info("Starting application");

            // Want to improve performance? Uncomment the lines below. Note though that this will disable
            // some features. 
            //
            // For more information, see http://docs.catelproject.com/vnext/faq/performance-considerations/

            // Log.Info("Improving performance");
            // Catel.Windows.Controls.UserControl.DefaultCreateWarningAndErrorValidatorForViewModelValue = false;
            // Catel.Windows.Controls.UserControl.DefaultSkipSearchingForInfoBarMessageControlValue = true;

            var directory = typeof(MainWindow).Assembly.GetDirectory();
            AppDomain.CurrentDomain.PreloadAssemblies(directory);

            Log.Info("Registering custom types");
            var serviceLocator = ServiceLocator.Default;

            serviceLocator.RegisterType<IStorageService, StorageService>();
            serviceLocator.RegisterType<IVideoProvider, YouTubeVideoProvider>();
            serviceLocator.RegisterType<IVideoChannelService, VideoChannelService>();
            serviceLocator.RegisterType<IVideoPlayerService, VideoPlayerService>();

            serviceLocator.RegisterType<ApplicationController>();

            serviceLocator.RegisterType<ApplicationModel>();

            // To auto-forward styles, check out Orchestra (see https://github.com/wildgums/orchestra)
            // StyleHelper.CreateStyleForwardersForDefaultStyles();

            Log.Info("Calling base.OnStartup");

            base.OnStartup(e);
        }

        protected override async void OnLoadCompleted(NavigationEventArgs e)
        {
            try
            {
                using (var mgr = UpdateManager.GitHubUpdateManager("https://github.com/relizoff/VSMC"))
                {
                    await mgr.Result.UpdateApp();
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex);
                MessageBox.Show(ex.Message);
            }

            base.OnLoadCompleted(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Get advisory report in console
            ApiCopManager.AddListener(new ConsoleApiCopListener());
            ApiCopManager.WriteResults();

            base.OnExit(e);
        }
    }
}