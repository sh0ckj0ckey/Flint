using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Data;
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



        #region MyGlossaries

        public async void InitBuildinGlossaries()
        {
            try
            {
                if (BuildinGlossaries != null && BuildinGlossaries?.Count > 0)
                {
                    return;
                }

                BuildinGlossaries?.Clear();
                BuildinGlossaries = new ObservableCollection<GlossaryBuildinModel>
                {
                    new()
                    {
                        GlossaryTitle = "牛津核心词汇",
                        BuildinGlossaryInternalTag = "oxford",
                        BuildinGlossaryIcon = "\uE825",
                        IsReadOnly = true,
                        GlossaryWordsCount = 3461
                    },
                    new()
                    {
                        GlossaryTitle = "雅思词汇",
                        BuildinGlossaryInternalTag = "ielts",
                        BuildinGlossaryIcon = "\uF7DB",
                        IsReadOnly = true,
                        GlossaryWordsCount = 5040
                    },
                    new()
                    {
                        GlossaryTitle = "托福词汇",
                        BuildinGlossaryInternalTag = "toefl",
                        BuildinGlossaryIcon = "\uF7DB",
                        IsReadOnly = true,
                        GlossaryWordsCount = 6974
                    },
                    new()
                    {
                        GlossaryTitle = "GRE 词汇",
                        BuildinGlossaryInternalTag = "gre",
                        BuildinGlossaryIcon = "\uF7DB",
                        IsReadOnly = true,
                        GlossaryWordsCount = 7504
                    },
                    new()
                    {
                        GlossaryTitle = "考研词汇",
                        BuildinGlossaryInternalTag = "ky",
                        BuildinGlossaryIcon = "\uE7BE",
                        IsReadOnly = true,
                        GlossaryWordsCount = 4801
                    },
                    new()
                    {
                        GlossaryTitle = "CET 6 词汇",
                        BuildinGlossaryInternalTag = "cet6",
                        BuildinGlossaryIcon = "\uE1D3",
                        IsReadOnly = true,
                        GlossaryWordsCount = 5407
                    },
                    new()
                    {
                        GlossaryTitle = "CET 4 词汇",
                        BuildinGlossaryInternalTag = "cet4",
                        BuildinGlossaryIcon = "\uE1D3",
                        IsReadOnly = true,
                        GlossaryWordsCount = 3849
                    },
                    new()
                    {
                        GlossaryTitle = "高考词汇",
                        BuildinGlossaryInternalTag = "gk",
                        BuildinGlossaryIcon = "\uE7BC",
                        IsReadOnly = true,
                        GlossaryWordsCount = 3677
                    },
                    new()
                    {
                        GlossaryTitle = "中考词汇",
                        BuildinGlossaryInternalTag = "zk",
                        BuildinGlossaryIcon = "\uE913",
                        IsReadOnly = true,
                        GlossaryWordsCount = 1603
                    },
                };

                //await Task.Run(() =>
                //{
                //    foreach (var item in BuildinGlossaries)
                //    {
                //        // 查找每个生词本的单词数量，并且生成描述
                //        var count = StarDictDataAccess.GetBuildinGlossaryWordCount(item.BuildinGlossaryInternalTag);
                //        Dispatcher.TryEnqueue(() =>
                //        {
                //            item.GlossaryWordsCount = count;
                //            item.GlossaryDescription = $"共 {item.GlossaryWordsCount} 个单词";
                //        });
                //    }
                //});
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void GetBuildinGlossaryWords()
        {
            var list = StarDictDataAccess.GetBuildinGlossaryWords("gre", -1, 200);
        }

        #endregion

        #region MyGlossaries

        public void InitMyGlossaries()
        {
            try
            {
                MyGlossaries?.Clear();

                // 从数据库中读取用户生词本
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

        #endregion
    }
}
