using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Data.Models;

namespace Flint3.Models
{
    public class GlossaryColorPercentagePair : ObservableObject
    {
        public GlossaryColorsEnum Color { get; private set; } = GlossaryColorsEnum.Transparent;

        private int _count = 0;
        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        private int _totalCount = 0;
        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        public GlossaryColorPercentagePair(GlossaryColorsEnum color, int count, int totalCount)
        {
            Color = color;
            Count = count;
            TotalCount = totalCount;
        }
    }

    public class GlossaryMyModel : GlossaryBaseModel
    {
        /// <summary>
        /// 生词本的单词颜色比例
        /// </summary>
        private ObservableCollection<GlossaryColorPercentagePair> _wordsColorPercentage = new ObservableCollection<GlossaryColorPercentagePair>();
        public ObservableCollection<GlossaryColorPercentagePair> WordsColorPercentage
        {
            get => _wordsColorPercentage;
            private set => SetProperty(ref _wordsColorPercentage, value);
        }
    }
}
