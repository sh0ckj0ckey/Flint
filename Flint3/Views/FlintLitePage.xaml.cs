using Flint3.Controls;
using Flint3.Data.Models;
using Flint3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FlintLitePage : Page
    {
        public MainViewModel ViewModel { get; set; } = null;

        private Flyout _addToGlossaryFlyout = null;

        private AddToGlossaryControl _addToGlossaryControl = null;

        public FlintLitePage()
        {
            ViewModel = MainViewModel.Instance;
            this.InitializeComponent();

            //ViewModel.LiteSearchResultWordItems.CollectionChanged += (_, e) =>
            //{
            //    try
            //    {
            //        App.FlintLiteWindow.Height = (e.NewItems != null && e.NewItems.Count > 0) ? 386 : 64;
            //    }
            //    catch { }
            //};
        }

        private void FlintLiteViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.FlintLiteWindow.Height = string.IsNullOrWhiteSpace(SearchTextBox?.Text) ? 64 : 386;
        }

        /// <summary>
        /// �ı���������ʱ���ڱ��������ʱֻ��ʾ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.FlintLiteWindow.Height = string.IsNullOrWhiteSpace(SearchTextBox?.Text) ? 64 : 386;
            MainViewModel.Instance.MatchWord(SearchTextBox?.Text, false);
        }

        /// <summary>
        /// �����ʾ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickShowMainWindow(object sender, RoutedEventArgs e)
        {
            App.ShowMainWindow();
        }

        /// <summary>
        /// ��ӵ��ʵ����ʱ�
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
