using System;
using System.Threading.Tasks;
using Flint3.Controls.KeyVisual;
using Flint3.Data;
using Flint3.Helpers;
using Flint3.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FlintPage : Page
    {
        private MainViewModel _viewModel = null;
        public FlintPage()
        {
            this.InitializeComponent();

            _viewModel = MainViewModel.Instance;

            MainViewModel.Instance.ActSwitchAppTheme?.Invoke();

            MainViewModel.Instance.ActFocusOnTextBox = () => { SearchTextBox?.Focus(FocusState.Keyboard); };

            // 加载数据库
            StarDictDataAccess.InitializeDatabase();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.MainWindow.SetTitleBar(AppTitleBar);
            TitleBarHelper.UpdateTitleBar(App.MainWindow, ActualTheme);

            SearchTextBox.Style = GetSearchTextBoxStyle(MainViewModel.Instance.AppSettings.SearchBoxStyle);
        }

        /// <summary>
        /// 获取搜索输入框样式
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        private Style GetSearchTextBoxStyle(int searchBoxStyle)
        {
            if (searchBoxStyle == 0)
            {
                return (Style)App.Current.Resources["ModernTextBoxStyle"];
            }
            else if (searchBoxStyle == 1)
            {
                return (Style)App.Current.Resources["RoundTextBoxStyle"];
            }
            else
            {
                return (Style)App.Current.Resources["ClassicTextBoxStyle"];
            }
        }

        /// <summary>
        /// 前往设置页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickSettingsButton(object sender, RoutedEventArgs e)
        {
            //await Task.Delay(100);
            this.Frame.Navigate(typeof(SettingsPage));
        }

        /// <summary>
        /// 即时搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender is TextBox tb && !string.IsNullOrWhiteSpace(tb?.Text))
                {
                    //_viewModel.QueryWord(tb.Text);
                    _viewModel.MatchWord(tb.Text);
                }
                else
                {
                    _viewModel.MatchWord(string.Empty);
                }
            }
            catch { }
        }

        /// <summary>
        /// 取得焦点后，选中所有文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchBoxGotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is TextBox tb && !string.IsNullOrEmpty(tb?.Text))
                {
                    tb.SelectAll();
                }
            }
            catch { }
        }

        /// <summary>
        /// 点击隐藏窗口到系统托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickHide(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.ActHideWindow?.Invoke();
        }

        /// <summary>
        /// 点击置顶窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickPin(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PinToggleButton?.IsChecked == true)
                {
                    MainViewModel.Instance.ActPinWindow?.Invoke(true);
                }
                else
                {
                    MainViewModel.Instance.ActPinWindow?.Invoke(false);
                }
            }
            catch
            {
                PinToggleButton.IsChecked = false;
                MainViewModel.Instance.ActPinWindow?.Invoke(false);
            }
        }
    }
}
