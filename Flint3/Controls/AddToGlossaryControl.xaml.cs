using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Data.Models;
using Flint3.Models;
using Flint3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Controls
{
    [INotifyPropertyChanged]
    public sealed partial class AddToGlossaryControl : UserControl
    {
        /// <summary>
        /// 上次选中的生词本 ID
        /// </summary>
        private static int _lastTimeSelectedGlossaryId = -1;

        private Action _hideAddingFlyout = null;

        private StarDictWordItem _addingWordItem = null;

        private GlossaryColorsEnum _addingWordColor = GlossaryColorsEnum.Transparent;

        private bool _updatingAvailableGlossaries = false;

        /// <summary>
        /// 当前添加的单词
        /// </summary>
        public StarDictWordItem AddingWordItem
        {
            get => _addingWordItem;
            set => SetProperty(ref _addingWordItem, value);
        }

        /// <summary>
        /// 当前添加的单词颜色
        /// </summary>
        public GlossaryColorsEnum AddingWordColor
        {
            get => _addingWordColor;
            set => SetProperty(ref _addingWordColor, value);
        }

        /// <summary>
        /// 是否正在检索当前添加单词在各个生词本中是否存在
        /// </summary>
        public bool UpdatingAvailableGlossaries
        {
            get => _updatingAvailableGlossaries;
            set => SetProperty(ref _updatingAvailableGlossaries, value);
        }

        /// <summary>
        /// 可以添加当前单词的生词本列表
        /// </summary>
        public ObservableCollection<GlossaryMyModel> AvailableGlossaries { get; } = new ObservableCollection<GlossaryMyModel>();

        public AddToGlossaryControl(Action hideAddingFlyout)
        {
            this.InitializeComponent();
            _hideAddingFlyout = hideAddingFlyout;
        }

        public async void PrepareToAddWord(StarDictWordItem item)
        {
            try
            {
                this.AddingWordItem = item;
                this.AddingWordColor = GlossaryColorsEnum.Transparent;
                WordColorScrollViewer?.ChangeView(0, 0, null, true);
                WordDescTextBox.Text = "";

                this.AvailableGlossaries.Clear();
                this.UpdatingAvailableGlossaries = true;
                var glossaries = await MainViewModel.Instance.GetGlossariesWithoutThisWord(this.AddingWordItem);

                int selectIndex = 0;
                foreach (var glossary in glossaries)
                {
                    this.AvailableGlossaries.Add(glossary);

                    if (_lastTimeSelectedGlossaryId >= 0 && glossary.Id == _lastTimeSelectedGlossaryId)
                    {
                        selectIndex = this.AvailableGlossaries.Count - 1;
                    }
                }

                this.UpdatingAvailableGlossaries = false;

                GlossaryComboBox.SelectedIndex = selectIndex;

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

                    this.AddingWordColor = colorsEnum;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        private void OnClickAddWord(object sender, RoutedEventArgs e)
        {
            try
            {
                var glossary = GlossaryComboBox.SelectedItem as GlossaryMyModel;
                _ = MainViewModel.Instance.AddWordToMyGlossary(
                    this.AddingWordItem.Id,
                    glossary.Id,
                    this.AddingWordItem.Word,
                    this.AddingWordItem.Phonetic,
                    this.AddingWordItem.Definition,
                    this.AddingWordItem.Translation,
                    this.AddingWordItem.Exchange,
                    WordDescTextBox.Text,
                    this.AddingWordColor);

                _hideAddingFlyout?.Invoke();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 记录上次选中的生词本 ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlossaryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is GlossaryMyModel glossary)
            {
                _lastTimeSelectedGlossaryId = glossary.Id;
            }
        }
    }
}
