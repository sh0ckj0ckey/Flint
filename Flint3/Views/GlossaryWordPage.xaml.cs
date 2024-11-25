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
using Flint3.Data.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GlossaryWordPage : Page
    {
        public MainViewModel ViewModel { get; set; } = null;

        public GlossaryWordPage()
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

        private void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteGlossaryWordFlyout.Hide();

                ViewModel.DeleteWordFromMyGlossary(ViewModel.SelectedGlossaryWord.Id);

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

                    if (MainViewModel.Instance.SelectedGlossaryWord.Color != colorsEnum)
                    {
                        ColorSetFlyout.Hide();
                        MainViewModel.Instance.SelectedGlossaryWord.Color = colorsEnum;

                        MainViewModel.Instance.UpdateWordFromMyGlossary(
                            MainViewModel.Instance.SelectedGlossaryWord.Id,
                            MainViewModel.Instance.SelectedGlossaryWord.Description,
                            MainViewModel.Instance.SelectedGlossaryWord.Color);
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        private void OnClickSaveEditDesc(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.SelectedGlossaryWord.Description = EditWordDescTextBox.Text;
            EditWordDescFlyout?.Hide();

            MainViewModel.Instance.UpdateWordFromMyGlossary(
                            MainViewModel.Instance.SelectedGlossaryWord.Id,
                            MainViewModel.Instance.SelectedGlossaryWord.Description,
                            MainViewModel.Instance.SelectedGlossaryWord.Color);
        }

        private void OnClickCancelEditDesc(object sender, RoutedEventArgs e)
        {
            EditWordDescTextBox.Text = MainViewModel.Instance.SelectedGlossaryWord.Description;
            EditWordDescFlyout?.Hide();
        }
    }
}
