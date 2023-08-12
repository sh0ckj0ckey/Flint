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
using CommunityToolkit.Labs.WinUI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GlossaryPage : Page
    {
        public MainViewModel ViewModel { get; set; } = null;
        public GlossaryPage()
        {
            this.InitializeComponent();

            ViewModel = MainViewModel.Instance;
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void OnClickCreateGlossary(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.CreateGlossary(AddGlossaryNameTextBox.Text, AddGlossaryDescTextBox.Text);
            AddGlossaryFlyout.Hide();
        }

        private void OnSelectGlossaryTab(object sender, SelectionChangedEventArgs e)
        {
            if (sender is Segmented tab && tab.SelectedIndex == 1)
            {
                MainViewModel.Instance.InitBuildinGlossaries();
            }
        }

        private void OnClickGoBuildinGlossary(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Models.GlossaryBuildinModel model)
            {
                MainViewModel.Instance.GoBuildinGlossary(model, 200);
                this.Frame.Navigate(typeof(GlossaryContentPage));
            }
        }
    }
}
