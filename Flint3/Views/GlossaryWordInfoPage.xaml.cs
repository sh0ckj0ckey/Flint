using System;
using Flint3.Data.Models;
using Flint3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GlossaryWordInfoPage : Page
    {
        public MainViewModel ViewModel { get; set; } = null;

        public GlossaryWordInfoPage()
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

            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteGlossaryWordFlyout.Hide();

                _ = MainViewModel.Instance.DeleteWordFromMyGlossary(MainViewModel.Instance.SelectedGlossaryWord.Id);

                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 修改单词的颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickSetWordColor(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    string tag = btn?.Tag?.ToString();
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

                    if (MainViewModel.Instance.SelectedGlossaryWord.Color != colorsEnum)
                    {
                        MainViewModel.Instance.SelectedGlossaryWord.Color = colorsEnum;
                        ColorSetFlyout?.Hide();
                        _ = MainViewModel.Instance.UpdateWordFromMyGlossary(
                            MainViewModel.Instance.SelectedGlossaryWord.Id,
                            MainViewModel.Instance.SelectedGlossaryWord.Description,
                            MainViewModel.Instance.SelectedGlossaryWord.Color);
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 修改单词的描述
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickSaveEditDesc(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.SelectedGlossaryWord.Description = EditWordDescTextBox.Text;
            EditWordDescFlyout?.Hide();
            _ = MainViewModel.Instance.UpdateWordFromMyGlossary(
                MainViewModel.Instance.SelectedGlossaryWord.Id,
                MainViewModel.Instance.SelectedGlossaryWord.Description,
                MainViewModel.Instance.SelectedGlossaryWord.Color);

        }

        /// <summary>
        /// 还原单词的描述
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCancelEditDesc(object sender, RoutedEventArgs e)
        {
            EditWordDescTextBox.Text = MainViewModel.Instance.SelectedGlossaryWord.Description;
            EditWordDescFlyout?.Hide();
        }
    }
}
