using System;
using Flint3.Data.Models;
using Flint3.Models;
using Flint3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Controls
{
    public sealed partial class AddToGlossaryControl : UserControl
    {
        public MainViewModel ViewModel { get; set; } = null;

        public AddToGlossaryControl()
        {
            this.InitializeComponent();

            ViewModel = MainViewModel.Instance;

            PopupShadow.Receivers.Add(AddToGlossaryPopupShadowReceiver);
        }

        public void UpdateControl()
        {
            try
            {
                GlossaryComboBox.SelectedIndex = -1;
                WordDescTextBox.Text = "";
                WordColorScrollViewer?.ScrollToHorizontalOffset(0);
                WordColorScrollViewer?.ScrollToVerticalOffset(0);
                MainViewModel.Instance.AddingWordColor = GlossaryColorsEnum.Transparent;

                MainViewModel.Instance.GetAddGlossariesList();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        private void OnClickAddingWordColor(object sender, RoutedEventArgs e)
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

                    MainViewModel.Instance.AddingWordColor = colorsEnum;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        private void OnClickAddWord(object sender, RoutedEventArgs e)
        {
            if (GlossaryComboBox.SelectedItem is GlossaryMyModel glossary && glossary is not null)
            {
                string description = WordDescTextBox.Text;

                var adding = MainViewModel.Instance.AddingWordItem;
                MainViewModel.Instance.AddWordToMyGlossary(
                    adding.Id,
                    glossary.Id,
                    adding.Word,
                    adding.Phonetic,
                    adding.Definition,
                    adding.Translation,
                    adding.Exchange,
                    description,
                    MainViewModel.Instance.AddingWordColor);

                MainViewModel.Instance.ActHideAddingPopup?.Invoke();
            }
        }
    }
}
