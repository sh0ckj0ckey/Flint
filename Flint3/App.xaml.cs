using System.Diagnostics;
using System.Threading.Tasks;
using Flint3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static new App Current => (App)Application.Current;

        /// <summary>
        /// 应用程序系统托盘
        /// </summary>
        public NotifyIcon NotifyIcon = null;

        /// <summary>
        /// 燧石的主窗口，点击桌面图标或者右下角托盘时一定打开这个窗口
        /// </summary>
        public MainWindow FlintMainWindow { get; private set; } = null;

        /// <summary>
        /// 燧石的简洁搜索窗口，设置中开启简洁窗口后，按下快捷键会唤起这个窗口
        /// </summary>
        public LiteWindow FlintLiteWindow { get; private set; } = null;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            MainViewModel.Instance.Initialize(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());

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
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            AppActivationArguments activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            if (activatedArgs?.Kind == ExtendedActivationKind.StartupTask)
            {
                MainViewModel.Instance.HideApp();
            }
            else
            {
                await ShowMainWindowFromRedirectAsync();
            }
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

            Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
            {
                MainViewModel.Instance.ShowMainWindow();
            });
        }
    }
}
