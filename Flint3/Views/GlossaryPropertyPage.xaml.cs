using System;
using CommunityToolkit.Mvvm.ComponentModel;
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
    [INotifyPropertyChanged]
    public sealed partial class GlossaryPropertyPage : Page
    {
        public MainViewModel ViewModel { get; set; } = null;

        private bool _editingGlossaryProperty = false;

        /// <summary>
        /// 当前是否正在编辑生词本属性
        /// </summary>
        public bool EditingGlossaryProperty
        {
            get => _editingGlossaryProperty;
            set => SetProperty(ref _editingGlossaryProperty, value);
        }

        public GlossaryPropertyPage()
        {
            ViewModel = MainViewModel.Instance;
            this.InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.EditingGlossaryProperty = false;
            if (!MainViewModel.Instance.SelectedGlossary.IsReadOnly)
            {
                // 只有非内置生词本才需要查询词汇数量
                UpdateGlossaryWordsCount();
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.EditingGlossaryProperty = false;
            EditGlossaryTitleTextBox.Text = MainViewModel.Instance.SelectedGlossary.GlossaryTitle;
            EditGlossaryDescTextBox.Text = MainViewModel.Instance.SelectedGlossary.GlossaryDescription;
        }

        /// <summary>
        /// 加载各种颜色的生词数量
        /// </summary>
        private async void UpdateGlossaryWordsCount()
        {
            int glossaryId = MainViewModel.Instance.SelectedGlossary.Id;
            var glossaryAllWordsCount = await MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Transparent);
            MainViewModel.Instance.SelectedGlossary.GlossaryWordsCount = glossaryAllWordsCount;

            if (MainViewModel.Instance.SelectedGlossary is Models.GlossaryMyModel glossary)
            {
                glossary.WordsColorPercentage.Clear();

                if (glossaryAllWordsCount > 0)
                {
                    var glossaryRedWordsCount = await MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Red);
                    if (glossaryRedWordsCount > 0) { glossary.WordsColorPercentage.Add(new(Data.Models.GlossaryColorsEnum.Red, glossaryRedWordsCount, glossaryAllWordsCount)); }

                    var glossaryOrgWordsCount = await MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Orange);
                    if (glossaryOrgWordsCount > 0) { glossary.WordsColorPercentage.Add(new(Data.Models.GlossaryColorsEnum.Orange, glossaryOrgWordsCount, glossaryAllWordsCount)); }

                    var glossaryYlwWordsCount = await MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Yellow);
                    if (glossaryYlwWordsCount > 0) { glossary.WordsColorPercentage.Add(new(Data.Models.GlossaryColorsEnum.Yellow, glossaryYlwWordsCount, glossaryAllWordsCount)); }

                    var glossaryGrnWordsCount = await MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Green);
                    if (glossaryGrnWordsCount > 0) { glossary.WordsColorPercentage.Add(new(Data.Models.GlossaryColorsEnum.Green, glossaryGrnWordsCount, glossaryAllWordsCount)); }

                    var glossaryBluWordsCount = await MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Blue);
                    if (glossaryBluWordsCount > 0) { glossary.WordsColorPercentage.Add(new(Data.Models.GlossaryColorsEnum.Blue, glossaryBluWordsCount, glossaryAllWordsCount)); }

                    var glossaryPplWordsCount = await MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Purple);
                    if (glossaryPplWordsCount > 0) { glossary.WordsColorPercentage.Add(new(Data.Models.GlossaryColorsEnum.Purple, glossaryPplWordsCount, glossaryAllWordsCount)); }

                    var glossaryPnkWordsCount = await MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Pink);
                    if (glossaryPnkWordsCount > 0) { glossary.WordsColorPercentage.Add(new(Data.Models.GlossaryColorsEnum.Pink, glossaryPnkWordsCount, glossaryAllWordsCount)); }

                    var glossaryBrnWordsCount = await MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Brown);
                    if (glossaryBrnWordsCount > 0) { glossary.WordsColorPercentage.Add(new(Data.Models.GlossaryColorsEnum.Brown, glossaryBrnWordsCount, glossaryAllWordsCount)); }

                    var glossaryGryWordsCount = await MainViewModel.Instance.GetWordsCountOfMyGlossary(glossaryId, Data.Models.GlossaryColorsEnum.Gray);
                    if (glossaryGryWordsCount > 0) { glossary.WordsColorPercentage.Add(new(Data.Models.GlossaryColorsEnum.Gray, glossaryGryWordsCount, glossaryAllWordsCount)); }
                }

                GlossaryColorPercentItemsRepeater.ItemsSource = glossary.WordsColorPercentage;
            }
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
            this.EditingGlossaryProperty = true;
            EditGlossaryTitleTextBox.Focus(FocusState.Keyboard);
        }

        private void OnClickSaveButton(object sender, RoutedEventArgs e)
        {
            this.EditingGlossaryProperty = false;
            MainViewModel.Instance.SelectedGlossary.GlossaryTitle = EditGlossaryTitleTextBox.Text;
            MainViewModel.Instance.SelectedGlossary.GlossaryDescription = EditGlossaryDescTextBox.Text;

            _ = MainViewModel.Instance.UpdateMyGlossary(ViewModel.SelectedGlossary.Id, ViewModel.SelectedGlossary.GlossaryTitle, ViewModel.SelectedGlossary.GlossaryDescription);
        }

        private void OnClickCancelButton(object sender, RoutedEventArgs e)
        {
            this.EditingGlossaryProperty = false;
            EditGlossaryTitleTextBox.Text = MainViewModel.Instance.SelectedGlossary.GlossaryTitle;
            EditGlossaryDescTextBox.Text = MainViewModel.Instance.SelectedGlossary.GlossaryDescription;
        }

        private async void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteGlossaryFlyout.Hide();

                await MainViewModel.Instance.DeleteMyGlossary(ViewModel.SelectedGlossary.Id);

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
