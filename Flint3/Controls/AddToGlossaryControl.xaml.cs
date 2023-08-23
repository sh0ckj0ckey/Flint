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
using Flint3.Models;

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
                WordColorScrollViewer?.ChangeView(0, null, null, true);

                MainViewModel.Instance.GetAddGlossariesList();
            }
            catch { }
        }

        private void OnClickAddingWordColor(object sender, RoutedEventArgs e)
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

                    MainViewModel.Instance.AddingWordColor = colorsEnum;
                }
            }
            catch { }
        }

        private void OnClickAddWord(object sender, RoutedEventArgs e)
        {
            if (GlossaryComboBox.SelectedItem is GlossaryMyModel glossary && glossary is not null)
            {
                string description = WordDescTextBox.Text;
                int color = (int)MainViewModel.Instance.AddingWordColor;

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
                    color);

                MainViewModel.Instance.ActHideAddingPopup?.Invoke();
            }
        }
    }
}
