using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Flint3.ViewModels;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GlossaryContentPage : Page
    {
        public MainViewModel ViewModel { get; set; } = null;
        private ScrollViewer _glossaryWordsScrollViewer = null;

        private SlideNavigationTransitionInfo SlideNaviTransition = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };

        public GlossaryContentPage()
        {
            this.InitializeComponent();

            this.Loaded += OnLoaded;
            GlossaryWordsListView.Loaded += OnGlossaryWordsListViewLoaded;
            GlossaryWordsListView.Unloaded += OnGlossaryWordsListViewUnloaded;

            ViewModel = MainViewModel.Instance;

            ViewModel.ActScrollToGlossaryTop = () =>
            {
                try
                {
                    _glossaryWordsScrollViewer?.ChangeView(0, 0, null, true);
                }
                catch { }
            };
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(FilterWordTextBox.Text))
                {
                    ViewModel?.GetFirstPageGlossaryWords();
                }
                else
                {
                    FilterWordTextBox.Text = "";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void OnGlossaryWordsListViewLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var listView = (ListView)sender;
                Border border = VisualTreeHelper.GetChild(listView, 0) as Border;
                _glossaryWordsScrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;

                if (_glossaryWordsScrollViewer != null)
                {
                    _glossaryWordsScrollViewer.ViewChanged += GlossaryWordsScrollViewer_ViewChanged;

                    Debug.WriteLine($"OnGlossaryWordsListViewLoaded ViewChanged Registered");
                }
            }
            catch { }
        }

        private void OnGlossaryWordsListViewUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_glossaryWordsScrollViewer != null)
                {
                    _glossaryWordsScrollViewer.ViewChanged -= GlossaryWordsScrollViewer_ViewChanged;
                    Debug.WriteLine($"OnGlossaryWordsListViewUnloaded ViewChanged Unregistered");
                }
            }
            catch { }
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void GlossaryWordsScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            try
            {
                if (!e.IsIntermediate)
                {
                    var scroller = (ScrollViewer)sender;
                    var distanceToEnd = scroller.ExtentHeight - (scroller.VerticalOffset + scroller.ViewportHeight);

                    if (distanceToEnd <= 20)
                    {
                        ViewModel.GetMoreGlossaryWords();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                string word = tb.Text;
                if (MainViewModel.Instance.FilterGlossaryWord != word)
                {
                    MainViewModel.Instance.FilterGlossaryWord = word;
                    ViewModel?.GetFirstPageGlossaryWords();
                }
            }
        }

        private void OnClickFilterColor(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.Tag is string tag)
                {
                    Models.GlossaryColorsEnum colorsEnum = Models.GlossaryColorsEnum.Transparent;
                    switch (tag)
                    {
                        case "0":
                            colorsEnum = Models.GlossaryColorsEnum.Transparent;
                            break;
                        case "1":
                            colorsEnum = Models.GlossaryColorsEnum.Red;
                            break;
                        case "2":
                            colorsEnum = Models.GlossaryColorsEnum.Orange;
                            break;
                        case "3":
                            colorsEnum = Models.GlossaryColorsEnum.Yellow;
                            break;
                        case "4":
                            colorsEnum = Models.GlossaryColorsEnum.Green;
                            break;
                        case "5":
                            colorsEnum = Models.GlossaryColorsEnum.Blue;
                            break;
                        case "6":
                            colorsEnum = Models.GlossaryColorsEnum.Purple;
                            break;
                        case "7":
                            colorsEnum = Models.GlossaryColorsEnum.Pink;
                            break;
                        case "8":
                            colorsEnum = Models.GlossaryColorsEnum.Brown;
                            break;
                        case "9":
                            colorsEnum = Models.GlossaryColorsEnum.Gray;
                            break;
                    }

                    if (MainViewModel.Instance.FilterGlossaryColor != colorsEnum)
                    {
                        ColorFilterFlyout.Hide();
                        MainViewModel.Instance.FilterGlossaryColor = colorsEnum;
                        ViewModel?.GetFirstPageGlossaryWords();
                    }
                }
            }
            catch { }
        }

        private void OnClickGlossaryProperty(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GlossaryPropertyPage), null, SlideNaviTransition);
        }
    }
}
