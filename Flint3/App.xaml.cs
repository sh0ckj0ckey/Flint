﻿using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Windows.System;
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
        private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

        public static WindowEx MainWindow { get; } = new MainWindow();

        // public static WindowEx LiteWindow { get; } = new FlintLiteWindow();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            UnhandledException += (s, e) =>
            {
                e.Handled = true;
                Debug.WriteLine(e.Message);
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
                MainWindow.Hide();
            }
            else
            {
                MainWindow.Activate();
            }

            // LiteWindow.Hide();
        }

        public async Task ShowMainWindowFromRedirectAsync()
        {
            while (MainWindow == null)
            {
                await Task.Delay(100);
            }

            _dispatcherQueue.TryEnqueue(() =>
            {
                MainWindow.Restore();
                MainWindow.CenterOnScreen();
                MainWindow.BringToFront();
                MainWindow.Activate();
            });
        }
    }
}
