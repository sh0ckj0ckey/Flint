using System.Diagnostics;
using System.Threading.Tasks;
using Flint3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            MainViewModel.Instance.Dispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            UnhandledException += (s, e) =>
            {
                e.Handled = true;
                Trace.WriteLine(e.Message);
            };
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            AppActivationArguments activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            if (activatedArgs?.Kind == ExtendedActivationKind.StartupTask)
            {
                MainViewModel.Instance.FlintMainWindow.Hide();
            }
            else
            {
                MainViewModel.Instance.FlintMainWindow.BringToFront();
                MainViewModel.Instance.FlintMainWindow.Activate();
            }

            MainViewModel.Instance.FlintLiteWindow?.Hide();
        }

        /// <summary>
        /// 重定向唤起主窗口
        /// </summary>
        /// <returns></returns>
        public async Task ShowMainWindowFromRedirectAsync()
        {
            while (MainViewModel.Instance.FlintMainWindow == null)
            {
                await Task.Delay(100);
            }

            MainViewModel.Instance.Dispatcher.TryEnqueue(() =>
            {
                MainViewModel.Instance.FlintLiteWindow?.Hide();
                MainViewModel.Instance.FlintMainWindow.Restore();
                MainViewModel.Instance.FlintMainWindow.CenterOnScreen();
                MainViewModel.Instance.FlintMainWindow.BringToFront();
                MainViewModel.Instance.FlintMainWindow.Activate();
            });
        }
    }
}
