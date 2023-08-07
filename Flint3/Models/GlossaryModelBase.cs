using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Flint3.Models
{
    public class GlossaryModelBase : ObservableObject
    {
        /// <summary>
        /// 是否只读，扩展生词本为只读的
        /// </summary>
        public bool IsReadOnly { get; set; } = false;

        /// <summary>
        /// 生词本的颜色
        /// </summary>
        private GlossaryColorsEnum _glossaryColor = GlossaryColorsEnum.Transparent;
        public GlossaryColorsEnum GlossaryColor
        {
            get => _glossaryColor;
            set => SetProperty(ref _glossaryColor, value);
        }

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
        /// 目前用来显示内置生词本内单词数量
        /// </summary>
        private string _glossaryDescription = string.Empty;
        public string GlossaryDescription
        {
            get => _glossaryDescription;
            set => SetProperty(ref _glossaryDescription, value);
        }
    }
}
