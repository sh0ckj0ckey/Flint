using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Flint3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GlossaryPropertyPage : Page
    {
        private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;
        public MainViewModel ViewModel { get; set; } = null;

        public GlossaryPropertyPage()
        {
            this.InitializeComponent();
            ViewModel = MainViewModel.Instance;
            _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.EditingGlossaryProperty = false;

            // 只有非内置生词本才需要查询词汇数量
            if (!MainViewModel.Instance.SelectedGlossary.IsReadOnly)
            {
                UpdateGlossaryWordsCount();
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.EditingGlossaryProperty = false;
            EditGlossaryTitleTextBox.Text = ViewModel.SelectedGlossary.GlossaryTitle;
            EditGlossaryDescTextBox.Text = ViewModel.SelectedGlossary.GlossaryDescription;
        }

        /// <summary>
        /// 加载各种颜色的生词数量
        /// </summary>
        private async void UpdateGlossaryWordsCount()
        {
            await Task.Run(() =>
            {
                int glossaryId = MainViewModel.Instance.SelectedGlossary.Id;
                var glossaryAllWordsCount = MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Transparent);
                var glossaryRedWordsCount = MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Red);
                var glossaryOrgWordsCount = MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Orange);
                var glossaryYlwWordsCount = MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Yellow);
                var glossaryGrnWordsCount = MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Green);
                var glossaryBluWordsCount = MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Blue);
                var glossaryPplWordsCount = MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Purple);
                var glossaryPnkWordsCount = MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Pink);
                var glossaryBrnWordsCount = MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Brown);
                var glossaryGryWordsCount = MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Gray);

                _dispatcherQueue.TryEnqueue(() =>
                {
                    MainViewModel.Instance.SelectedGlossary.GlossaryWordsCount = glossaryAllWordsCount;
                    if (MainViewModel.Instance.SelectedGlossary is Models.GlossaryMyModel glossary)
                    {
                        glossary.WordsColorPercentage.Clear();

                        if (glossaryAllWordsCount > 0)
                        {
                            if (glossaryRedWordsCount > 0) { glossary.WordsColorPercentage.Add(new Models.GlossaryColorPercentagePair(Data.Models.GlossaryColorsEnum.Red, glossaryRedWordsCount, glossaryAllWordsCount)); }
                            if (glossaryOrgWordsCount > 0) { glossary.WordsColorPercentage.Add(new Models.GlossaryColorPercentagePair(Data.Models.GlossaryColorsEnum.Orange, glossaryOrgWordsCount, glossaryAllWordsCount)); }
                            if (glossaryYlwWordsCount > 0) { glossary.WordsColorPercentage.Add(new Models.GlossaryColorPercentagePair(Data.Models.GlossaryColorsEnum.Yellow, glossaryYlwWordsCount, glossaryAllWordsCount)); }
                            if (glossaryGrnWordsCount > 0) { glossary.WordsColorPercentage.Add(new Models.GlossaryColorPercentagePair(Data.Models.GlossaryColorsEnum.Green, glossaryGrnWordsCount, glossaryAllWordsCount)); }
                            if (glossaryBluWordsCount > 0) { glossary.WordsColorPercentage.Add(new Models.GlossaryColorPercentagePair(Data.Models.GlossaryColorsEnum.Blue, glossaryBluWordsCount, glossaryAllWordsCount)); }
                            if (glossaryPplWordsCount > 0) { glossary.WordsColorPercentage.Add(new Models.GlossaryColorPercentagePair(Data.Models.GlossaryColorsEnum.Purple, glossaryPplWordsCount, glossaryAllWordsCount)); }
                            if (glossaryPnkWordsCount > 0) { glossary.WordsColorPercentage.Add(new Models.GlossaryColorPercentagePair(Data.Models.GlossaryColorsEnum.Pink, glossaryPnkWordsCount, glossaryAllWordsCount)); }
                            if (glossaryBrnWordsCount > 0) { glossary.WordsColorPercentage.Add(new Models.GlossaryColorPercentagePair(Data.Models.GlossaryColorsEnum.Brown, glossaryBrnWordsCount, glossaryAllWordsCount)); }
                            if (glossaryGryWordsCount > 0) { glossary.WordsColorPercentage.Add(new Models.GlossaryColorPercentagePair(Data.Models.GlossaryColorsEnum.Gray, glossaryGryWordsCount, glossaryAllWordsCount)); }

                            GlossaryColorPercentItemsRepeater.ItemsSource = glossary.WordsColorPercentage;
                        }
                    }
                });
            });
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void OnClickEditButton(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.EditingGlossaryProperty = true;
            EditGlossaryTitleTextBox.Focus(FocusState.Pointer);
        }

        private void OnClickSaveButton(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.EditingGlossaryProperty = false;
            ViewModel.SelectedGlossary.GlossaryTitle = EditGlossaryTitleTextBox.Text;
            ViewModel.SelectedGlossary.GlossaryDescription = EditGlossaryDescTextBox.Text;

            ViewModel.UpdateMyGlossary(ViewModel.SelectedGlossary.Id, ViewModel.SelectedGlossary.GlossaryTitle, ViewModel.SelectedGlossary.GlossaryDescription);
        }

        private void OnClickCancelButton(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.EditingGlossaryProperty = false;
            EditGlossaryTitleTextBox.Text = ViewModel.SelectedGlossary.GlossaryTitle;
            EditGlossaryDescTextBox.Text = ViewModel.SelectedGlossary.GlossaryDescription;
        }

        private void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteGlossaryFlyout.Hide();

                ViewModel.DeleteMyGlossary(ViewModel.SelectedGlossary.Id);

                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }

                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }
    }
}
