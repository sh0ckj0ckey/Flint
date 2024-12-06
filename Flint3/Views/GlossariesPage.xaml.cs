using Flint3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GlossariesPage : Page
    {
        private SlideNavigationTransitionInfo SlideNaviTransition = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };

        public MainViewModel ViewModel { get; set; } = null;

        public GlossariesPage()
        {
            ViewModel = MainViewModel.Instance;
            this.InitializeComponent();
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
            _ = MainViewModel.Instance.AddMyGlossary(AddGlossaryNameTextBox.Text, AddGlossaryDescTextBox.Text);
            AddGlossaryFlyout.Hide();
            AddGlossaryNameTextBox.Text = "";
            AddGlossaryDescTextBox.Text = "";
        }

        private void OnClickGoMyGlossary(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Models.GlossaryMyModel model)
            {
                MainViewModel.Instance.SelectGlossary(model);
                this.Frame.Navigate(typeof(GlossaryWordsPage), null, SlideNaviTransition);
            }
        }

        private void OnClickGoExGlossary(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Models.GlossaryExModel model)
            {
                MainViewModel.Instance.SelectGlossary(model);
                this.Frame.Navigate(typeof(GlossaryWordsPage), null, SlideNaviTransition);
            }
        }
    }
}
