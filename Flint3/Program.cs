using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;

namespace Flint3
{
    /// <summary>
    /// 单实例，参考自 https://github.com/marticliment/WingetUI
    /// </summary>
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                _ = AsyncMain(args);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        private static async Task AsyncMain(string[] args)
        {
            try
            {
                // WinRT single-instance fancy stuff
                WinRT.ComWrappersSupport.InitializeComWrappers();
                bool isRedirect = await DecideRedirection();
                if (!isRedirect) // Sometimes, redirection fails, so we try again
                    isRedirect = await DecideRedirection();
                if (!isRedirect) // Sometimes, redirection fails, so we try again (second time)
                    isRedirect = await DecideRedirection();

                // If this is the main instance, start the app
                if (!isRedirect)
                {
                    Microsoft.UI.Xaml.Application.Start((p) =>
                    {
                        DispatcherQueueSynchronizationContext context = new(DispatcherQueue.GetForCurrentThread());
                        SynchronizationContext.SetSynchronizationContext(context);
                        _ = new App();
                    });
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        private static async Task<bool> DecideRedirection()
        {
            bool isRedirect = false;

            try
            {
                AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
                ExtendedActivationKind kind = args.Kind;

                AppInstance keyInstance = AppInstance.FindOrRegisterForKey("NoMewing.sh0ckj0ckey.Flint3");

                if (keyInstance.IsCurrent)
                {
                    keyInstance.Activated += async (s, e) =>
                    {
                        App appInstance = App.Current as App;
                        await appInstance?.ShowMainWindowFromRedirectAsync();
                    };
                }
                else
                {
                    isRedirect = true;
                    await keyInstance.RedirectActivationToAsync(args);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }

            return isRedirect;
        }
    }
}
