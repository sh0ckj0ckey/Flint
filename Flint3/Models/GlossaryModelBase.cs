using CommunityToolkit.Mvvm.ComponentModel;

namespace Flint3.Models
{
    public class GlossaryModelBase : ObservableObject
    {
        public int Id { get; set; } = -1;

        /// <summary>
        /// 是否只读，扩展生词本为只读的
        /// </summary>
        public bool IsReadOnly { get; set; } = false;

        /// <summary>
        /// 生词本名称
        /// </summary>
        private string _glossaryTitle = string.Empty;
        public string GlossaryTitle
        {
            get => _glossaryTitle;
            set => SetProperty(ref _glossaryTitle, value);
        }

        /// <summary>
        /// 描述
        /// </summary>
        private string _glossaryDescription = string.Empty;
        public string GlossaryDescription
        {
            get => _glossaryDescription;
            set => SetProperty(ref _glossaryDescription, value);
        }

        /// <summary>
        /// 单词数量
        /// </summary>
        private int _glossaryWordsCount = 0;
        public int GlossaryWordsCount
        {
            get => _glossaryWordsCount;
            set => SetProperty(ref _glossaryWordsCount, value);
        }

        /// <summary>
        /// 生词本的图标
        /// </summary>
        public string GlossaryIcon { get; set; } = string.Empty;
    }
}
