using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Data.Models;
using Flint3.Models;

namespace Flint3.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// 内置生词本
        /// </summary>
        private ObservableCollection<GlossaryBuildinModel> _buildinGlossaries = null;
        public ObservableCollection<GlossaryBuildinModel> BuildinGlossaries
        {
            get => _buildinGlossaries;
            set => SetProperty(ref _buildinGlossaries, value);
        }

        private void InitBuildinGlossaries()
        {
            try
            {
                BuildinGlossaries?.Clear();
                BuildinGlossaries = new ObservableCollection<GlossaryBuildinModel>
                {
                    new()
                    {
                        GlossaryTitle = "牛津核心词汇",
                        GlossaryDescription = "共 3000 个单词",
                        BuildinGlossaryInternalTag = "oxford",
                        BuildinGlossaryIcon = "\uE825",
                        GlossaryColor = GlossaryColorsEnum.Purple
                    },
                    new()
                    {
                        GlossaryTitle = "雅思词汇",
                        GlossaryDescription = "共 3000 个单词",
                        BuildinGlossaryInternalTag = "ielts",
                        BuildinGlossaryIcon = "\uF7DB",
                        GlossaryColor = GlossaryColorsEnum.Green
                    },
                    new()
                    {
                        GlossaryTitle = "托福词汇",
                        GlossaryDescription = "共 3000 个单词",
                        BuildinGlossaryInternalTag = "toefl",
                        BuildinGlossaryIcon = "\uF7DB",
                        GlossaryColor = GlossaryColorsEnum.Green
                    },
                    new()
                    {
                        GlossaryTitle = "GRE 词汇",
                        GlossaryDescription = "共 3000 个单词",
                        BuildinGlossaryInternalTag = "gre",
                        BuildinGlossaryIcon = "\uF7DB",
                        GlossaryColor = GlossaryColorsEnum.Green
                    },
                    new()
                    {
                        GlossaryTitle = "考研词汇",
                        GlossaryDescription = "共 3000 个单词",
                        BuildinGlossaryInternalTag = "ky",
                        BuildinGlossaryIcon = "\uE7BE",
                        GlossaryColor = GlossaryColorsEnum.Red
                    },
                    new()
                    {
                        GlossaryTitle = "CET 6 词汇",
                        GlossaryDescription = "共 3000 个单词",
                        BuildinGlossaryInternalTag = "cet6",
                        BuildinGlossaryIcon = "\uE1D3",
                        GlossaryColor = GlossaryColorsEnum.Blue
                    },
                    new()
                    {
                        GlossaryTitle = "CET 4 词汇",
                        GlossaryDescription = "共 3000 个单词",
                        BuildinGlossaryInternalTag = "cet6",
                        BuildinGlossaryIcon = "\uE1D3",
                        GlossaryColor = GlossaryColorsEnum.Blue
                    },
                    new()
                    {
                        GlossaryTitle = "高考词汇",
                        GlossaryDescription = "共 3000 个单词",
                        BuildinGlossaryInternalTag = "gk",
                        BuildinGlossaryIcon = "\uE7BC",
                        GlossaryColor = GlossaryColorsEnum.Pink
                    },
                    new()
                    {
                        GlossaryTitle = "中考词汇",
                        GlossaryDescription = "共 3000 个单词",
                        BuildinGlossaryInternalTag = "zk",
                        BuildinGlossaryIcon = "\uE913",
                        GlossaryColor = GlossaryColorsEnum.Orange
                    },
                };
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
