using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        /// 内置生词本列表
        /// </summary>
        public ObservableCollection<GlossaryBuildinModel> BuildinGlossaries { get; private set; } = new ObservableCollection<GlossaryBuildinModel>();

        /// <summary>
        /// 用户生词本列表
        /// </summary>
        public ObservableCollection<GlossaryItemModel> MyGlossaries { get; private set; } = new ObservableCollection<GlossaryItemModel>();

        /// <summary>
        /// 当前正在查看的生词本
        /// </summary>
        private GlossaryModelBase _selectedGlossary = null;
        public GlossaryModelBase SelectedGlossary
        {
            get => _selectedGlossary;
            set => SetProperty(ref _selectedGlossary, value);
        }

        /// <summary>
        /// 当前选择的生词本单词列表
        /// </summary>
        public ObservableCollection<StarDictWordItem> GlossaryWordItems { get; private set; } = new ObservableCollection<StarDictWordItem>();


        #region BuildinGlossaries

        /// <summary>
        /// 初始化内置生词本
        /// </summary>
        public void InitBuildinGlossaries()
        {
            try
            {
                if (BuildinGlossaries?.Count > 0)
                {
                    return;
                }

                BuildinGlossaries?.Clear();

                BuildinGlossaries.Add(new()
                {
                    GlossaryTitle = "牛津核心词汇",
                    BuildinGlossaryInternalTag = "oxford",
                    GlossaryIcon = "\uE825",
                    IsReadOnly = true,
                    GlossaryWordsCount = 3461
                });

                BuildinGlossaries.Add(new()
                {
                    GlossaryTitle = "雅思词汇",
                    BuildinGlossaryInternalTag = "ielts",
                    GlossaryIcon = "\uF7DB",
                    IsReadOnly = true,
                    GlossaryWordsCount = 5040
                });

                BuildinGlossaries.Add(new()
                {
                    GlossaryTitle = "托福词汇",
                    BuildinGlossaryInternalTag = "toefl",
                    GlossaryIcon = "\uF7DB",
                    IsReadOnly = true,
                    GlossaryWordsCount = 6974
                });

                BuildinGlossaries.Add(new()
                {
                    GlossaryTitle = "GRE 词汇",
                    BuildinGlossaryInternalTag = "gre",
                    GlossaryIcon = "\uF7DB",
                    IsReadOnly = true,
                    GlossaryWordsCount = 7504
                });

                BuildinGlossaries.Add(new()
                {
                    GlossaryTitle = "考研词汇",
                    BuildinGlossaryInternalTag = "ky",
                    GlossaryIcon = "\uE7BE",
                    IsReadOnly = true,
                    GlossaryWordsCount = 4801
                });

                BuildinGlossaries.Add(new()
                {
                    GlossaryTitle = "CET 6 词汇",
                    BuildinGlossaryInternalTag = "cet6",
                    GlossaryIcon = "\uE1D3",
                    IsReadOnly = true,
                    GlossaryWordsCount = 5407
                });

                BuildinGlossaries.Add(new()
                {
                    GlossaryTitle = "CET 4 词汇",
                    BuildinGlossaryInternalTag = "cet4",
                    GlossaryIcon = "\uE1D3",
                    IsReadOnly = true,
                    GlossaryWordsCount = 3849
                });

                BuildinGlossaries.Add(new()
                {
                    GlossaryTitle = "高考词汇",
                    BuildinGlossaryInternalTag = "gk",
                    GlossaryIcon = "\uE7BC",
                    IsReadOnly = true,
                    GlossaryWordsCount = 3677
                });

                BuildinGlossaries.Add(new()
                {
                    GlossaryTitle = "中考词汇",
                    BuildinGlossaryInternalTag = "zk",
                    GlossaryIcon = "\uE913",
                    IsReadOnly = true,
                    GlossaryWordsCount = 1603
                });

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

        /// <summary>
        /// 选择某一个内置生词本，加载第一页单词
        /// </summary>
        /// <param name="model"></param>
        /// <param name="count"></param>
        public void GoBuildinGlossary(Models.GlossaryBuildinModel model, int count = 200)
        {
            GlossaryWordItems.Clear();
            SelectedGlossary = model;
            var list = StarDictDataAccess.GetBuildinGlossaryWords(model.BuildinGlossaryInternalTag, -1, count);

            foreach (var item in list)
            {
                GlossaryWordItems.Add(MakeupWord(item));
            }
        }

        /// <summary>
        /// 增量加载内置生词本的单词
        /// </summary>
        /// <param name="count"></param>
        public void IncreaseBuildinGlossaryWords(int count = 100)
        {
            if (SelectedGlossary is GlossaryBuildinModel glossary)
            {
                long lastId = GlossaryWordItems.Count > 0 ? GlossaryWordItems.Last().Id : -1;
                var list = StarDictDataAccess.GetBuildinGlossaryWords(glossary.BuildinGlossaryInternalTag, lastId, count);

                foreach (var item in list)
                {
                    GlossaryWordItems.Add(MakeupWord(item));
                }
            }
        }

        #endregion

        #region MyGlossaries

        /// <summary>
        /// 加载我的生词本
        /// </summary>
        public void InitMyGlossaries()
        {
            try
            {
                if (MyGlossaries?.Count > 0)
                {
                    return;
                }

                MyGlossaries?.Clear();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 创建新的生词本
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        public void CreateGlossary(string name, string desc)
        {
            try
            {
                MyGlossaries.Add(new GlossaryItemModel() { GlossaryTitle = name, GlossaryDescription = desc, GlossaryIcon = "\uEE56" });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        #endregion
    }
}
