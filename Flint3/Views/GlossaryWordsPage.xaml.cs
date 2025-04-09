using System;
using Flint3.Data.Models;
using Flint3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GlossaryWordsPage : Page
    {
        public MainViewModel ViewModel { get; set; } = null;

        private ScrollViewer _glossaryWordsScrollViewer = null;

        private SlideNavigationTransitionInfo SlideNaviTransition = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };

        public GlossaryWordsPage()
        {
            ViewModel = MainViewModel.Instance;
            this.InitializeComponent();
            GlossaryWordsListView.Loaded += OnGlossaryWordsListViewLoaded;
            GlossaryWordsListView.Unloaded += OnGlossaryWordsListViewUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                FilterWordTextBox.TextChanged -= OnFilterTextChanged;
                FilterWordTextBox.Text = MainViewModel.Instance.FilterGlossaryWord;
                FilterWordTextBox.TextChanged += OnFilterTextChanged;
                ScrollGlossaryWordsListViewToTop();
                _ = MainViewModel.Instance.GetAllGlossaryWords();

                UpdateHideButtonIcon();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private void OnGlossaryWordsListViewLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var listView = (ListView)sender;
                Border border = VisualTreeHelper.GetChild(listView, 0) as Border;
                _glossaryWordsScrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;

                //if (_glossaryWordsScrollViewer != null)
                //{
                //    _glossaryWordsScrollViewer.ViewChanged += GlossaryWordsScrollViewer_ViewChanged;
                //    System.Diagnostics.Trace.WriteLine($"OnGlossaryWordsListViewLoaded ViewChanged Registered");
                //}
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        private void OnGlossaryWordsListViewUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (_glossaryWordsScrollViewer != null)
                //{
                //    _glossaryWordsScrollViewer.ViewChanged -= GlossaryWordsScrollViewer_ViewChanged;
                //    System.Diagnostics.Trace.WriteLine($"OnGlossaryWordsListViewUnloaded ViewChanged Unregistered");
                //}
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        private void ScrollGlossaryWordsListViewToTop()
        {
            try
            {
                if (_glossaryWordsScrollViewer is not null)
                {
                    _glossaryWordsScrollViewer?.ChangeView(0, 0, null, true);
                }
                else
                {
                    if (GlossaryWordsListView.Items.Count > 0)
                    {
                        GlossaryWordsListView.ScrollIntoView(GlossaryWordsListView.Items[0]);
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
        private void OnClickBackHomeButton(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }

            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        /// <summary>
        /// 搜索词汇
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox filterTextBox)
            {
                string word = filterTextBox.Text;
                if (MainViewModel.Instance.FilterGlossaryWord != word)
                {
                    MainViewModel.Instance.FilterGlossaryWord = word;
                    ScrollGlossaryWordsListViewToTop();
                    _ = MainViewModel.Instance.GetAllGlossaryWords();
                }
            }
        }

        /// <summary>
        /// 根据颜色筛选词汇
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickFilterColor(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.Tag is string tag)
                {
                    GlossaryColorsEnum colorsEnum = GlossaryColorsEnum.Transparent;
                    switch (tag)
                    {
                        case "0":
                            colorsEnum = GlossaryColorsEnum.Transparent;
                            break;
                        case "1":
                            colorsEnum = GlossaryColorsEnum.Red;
                            break;
                        case "2":
                            colorsEnum = GlossaryColorsEnum.Orange;
                            break;
                        case "3":
                            colorsEnum = GlossaryColorsEnum.Yellow;
                            break;
                        case "4":
                            colorsEnum = GlossaryColorsEnum.Green;
                            break;
                        case "5":
                            colorsEnum = GlossaryColorsEnum.Blue;
                            break;
                        case "6":
                            colorsEnum = GlossaryColorsEnum.Purple;
                            break;
                        case "7":
                            colorsEnum = GlossaryColorsEnum.Pink;
                            break;
                        case "8":
                            colorsEnum = GlossaryColorsEnum.Brown;
                            break;
                        case "9":
                            colorsEnum = GlossaryColorsEnum.Gray;
                            break;
                    }

                    if (MainViewModel.Instance.FilterGlossaryColor != colorsEnum)
                    {
                        MainViewModel.Instance.FilterGlossaryColor = colorsEnum;
                        ColorFilterFlyout?.Hide();
                        ScrollGlossaryWordsListViewToTop();
                        _ = MainViewModel.Instance.GetAllGlossaryWords();
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 切换到字母排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCheckOrderByAlphabet(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainViewModel.Instance.GlossaryWordsOrderMode != 0)
                {
                    MainViewModel.Instance.GlossaryWordsOrderMode = 0;
                    ScrollGlossaryWordsListViewToTop();
                    _ = MainViewModel.Instance.GetAllGlossaryWords();
                }

                if (sender is ToggleMenuFlyoutItem menuItem)
                {
                    menuItem.IsChecked = true;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 切换到日期排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCheckOrderByDate(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainViewModel.Instance.GlossaryWordsOrderMode != 1)
                {
                    MainViewModel.Instance.GlossaryWordsOrderMode = 1;
                    ScrollGlossaryWordsListViewToTop();
                    _ = MainViewModel.Instance.GetAllGlossaryWords();
                }

                if (sender is ToggleMenuFlyoutItem menuItem)
                {
                    menuItem.IsChecked = true;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 切换隐藏单词或者释义
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickHideMenuItem(object sender, RoutedEventArgs e)
        {
            UpdateHideButtonIcon();
        }

        /// <summary>
        /// 根据菜单勾选情况更新隐藏按钮图标
        /// </summary>
        private void UpdateHideButtonIcon()
        {
            var hidingWord = HideWordToggleMenuItem.IsChecked;
            var hidingExplain = HideExplainToggleMenuItem.IsChecked;
            NoHideFontIcon.Visibility = !hidingWord && !hidingExplain ? Visibility.Visible : Visibility.Collapsed;
            HidingFontIcon.Visibility = (hidingWord || hidingExplain) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 查看生词本属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickGlossaryProperty(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GlossaryPropertyPage), null, SlideNaviTransition);
        }

        /// <summary>
        /// 点击单词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickGlossaryWord(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is StarDictWordItem item)
            {
                MainViewModel.Instance.SelectGlossaryWord(item);
                this.Frame.Navigate(typeof(GlossaryWordInfoPage), null, SlideNaviTransition);
            }
        }

        #region 增量加载

        /// <summary>
        /// 滚动到底部时增量加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void GlossaryWordsScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (!e.IsIntermediate)
        //        {
        //            var scroller = (ScrollViewer)sender;
        //            var distanceToEnd = scroller.ExtentHeight - (scroller.VerticalOffset + scroller.ViewportHeight);

        //            if (distanceToEnd <= 20)
        //            {
        //                MainViewModel.Instance.GetMoreGlossaryWords();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Trace.WriteLine(ex.Message);
        //    }
        //}

        #endregion

    }
}
