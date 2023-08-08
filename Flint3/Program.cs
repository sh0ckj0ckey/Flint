using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;
using Windows.Storage;

namespace Flint3
{
    public static class Program
    {
        private static App _app;

        [STAThread]
        public static void Main(string[] args)
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();

            var isRedirect = DecideRedirection().GetAwaiter().GetResult();

            if (!isRedirect)
            {
                Microsoft.UI.Xaml.Application.Start((p) =>
                {
                    var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                    var context = new DispatcherQueueSynchronizationContext(dispatcherQueue);
                    SynchronizationContext.SetSynchronizationContext(context);
                    _app = new App();
                });
            }
        }

        private static async Task<bool> DecideRedirection()
        {
            AppInstance keyInstance = AppInstance.FindOrRegisterForKey("FLINT3-B7D6DF43-BB1D-4C3B-9D57-9988C40BAC5A");
            AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();

            bool isRedirect = false;
            if (keyInstance.IsCurrent)
            {
                keyInstance.Activated += OnActivated;
            }
            else
            {
                isRedirect = true;
                await keyInstance.RedirectActivationToAsync(args);
            }
            return isRedirect;
        }

        private static void OnActivated(object sender, AppActivationArguments args)
        {
            _app?.ShowMainWindow();
        }
    }
}
