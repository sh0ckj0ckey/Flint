﻿using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Flint3.Data.Models
{
    public class StarDictWordItem : ObservableObject
    {
        public long Id { get; set; } = 0;

        /// <summary>
        /// 单词名称
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// 单词字符串经过 strip 以后的结果(去除整个字符串中非字母和数字的部分)，用于模糊匹配
        /// </summary>
        public string StripWord { get; set; }

        /// <summary>
        /// 音标，以英语英标为主
        /// </summary>
        public string Phonetic { get; set; }

        /// <summary>
        /// 单词释义（英文），每行一个释义
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// 单词释义（中文），每行一个释义
        /// </summary>
        public string Translation { get; set; }

        /// <summary>
        /// 时态复数等变换，使用 "/" 分割不同项目
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// 变换处理之后生成的集合
        /// </summary>
        public List<WordExchangeItem> Exchanges { get; set; } = null;

        /// <summary>
        /// 生词本备注
        /// </summary>
        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        /// <summary>
        /// 生词本颜色
        /// </summary>
        private GlossaryColorsEnum _color = GlossaryColorsEnum.Transparent;
        public GlossaryColorsEnum Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
    }

    public class WordExchangeItem
    {
        public string Type { get; set; }
        public string Word { get; set; }

        public WordExchangeItem(string word, string type)
        {
            Type = type;
            Word = word;
        }
    }
}
