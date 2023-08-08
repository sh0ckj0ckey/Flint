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

        /// <summary>
        /// 用户生词本
        /// </summary>
        private ObservableCollection<GlossaryItemModel> _myGlossaries = null;
        public ObservableCollection<GlossaryItemModel> MyGlossaries
        {
            get => _myGlossaries;
            set => SetProperty(ref _myGlossaries, value);
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
                        BuildinGlossaryInternalTag = "oxford",
                        BuildinGlossaryIcon = "\uE825",
                        IsReadOnly = true
                    },
                    new()
                    {
                        GlossaryTitle = "雅思词汇",
                        BuildinGlossaryInternalTag = "ielts",
                        BuildinGlossaryIcon = "\uF7DB",
                        IsReadOnly = true
                    },
                    new()
                    {
                        GlossaryTitle = "托福词汇",
                        BuildinGlossaryInternalTag = "toefl",
                        BuildinGlossaryIcon = "\uF7DB",
                        IsReadOnly = true
                    },
                    new()
                    {
                        GlossaryTitle = "GRE 词汇",
                        BuildinGlossaryInternalTag = "gre",
                        BuildinGlossaryIcon = "\uF7DB",
                        IsReadOnly = true
                    },
                    new()
                    {
                        GlossaryTitle = "考研词汇",
                        BuildinGlossaryInternalTag = "ky",
                        BuildinGlossaryIcon = "\uE7BE",
                        IsReadOnly = true
                    },
                    new()
                    {
                        GlossaryTitle = "CET 6 词汇",
                        BuildinGlossaryInternalTag = "cet6",
                        BuildinGlossaryIcon = "\uE1D3",
                        IsReadOnly = true
                    },
                    new()
                    {
                        GlossaryTitle = "CET 4 词汇",
                        BuildinGlossaryInternalTag = "cet6",
                        BuildinGlossaryIcon = "\uE1D3",
                        IsReadOnly = true
                    },
                    new()
                    {
                        GlossaryTitle = "高考词汇",
                        BuildinGlossaryInternalTag = "gk",
                        BuildinGlossaryIcon = "\uE7BC",
                        IsReadOnly = true
                    },
                    new()
                    {
                        GlossaryTitle = "中考词汇",
                        BuildinGlossaryInternalTag = "zk",
                        BuildinGlossaryIcon = "\uE913",
                        IsReadOnly = true
                    },
                };

                foreach (var item in BuildinGlossaries)
                {
                    // 查找每个生词本的单词数量，并且生成描述
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void InitMyGlossaries()
        {
            try
            {
                MyGlossaries?.Clear();
                MyGlossaries = new ObservableCollection<GlossaryItemModel>();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void CreateGlossary(string name, string desc)
        {
            try
            {
                MyGlossaries.Add(new GlossaryItemModel() { GlossaryTitle = name, GlossaryDescription = desc });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
