using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Flint3.Controls;
using Flint3.Data.Models;
using Flint3.ViewModels;
using System.Runtime.InteropServices;
using WinUIEx;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using System.Drawing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LitePage : Page
    {
        public MainViewModel ViewModel { get; set; } = null;

        private Flyout _addToGlossaryFlyout = null;

        private AddToGlossaryControl _addToGlossaryControl = null;

        public LitePage()
        {
            ViewModel = MainViewModel.Instance;
            this.InitializeComponent();
        }

        /// <summary>
        /// 文本框有内容时窗口变大，无内容时只显示搜索框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MainViewModel.Instance.FlintLiteWindow.Height = string.IsNullOrWhiteSpace(SearchTextBox?.Text) ? 64 : 386;
            MainViewModel.Instance.MatchWord(SearchTextBox?.Text, false);
        }

        /// <summary>
        /// 点击显示主窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickShowMainWindow(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.ShowMainWindow();
        }

        /// <summary>
        /// 添加单词到生词本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickAddWordToGlossary(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is StarDictWordItem item)
            {
                _addToGlossaryControl ??= new AddToGlossaryControl(() => { _addToGlossaryFlyout?.Hide(); });
                _addToGlossaryFlyout ??= new Flyout() { Content = _addToGlossaryControl };
                _addToGlossaryControl.PrepareToAddWord(item);

                FlyoutBase.SetAttachedFlyout(btn, _addToGlossaryFlyout);
                FlyoutBase.ShowAttachedFlyout(btn);
            }
        }

    }
}
