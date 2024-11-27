using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Core;
using Flint3.Data;
using Flint3.Data.Models;
using Flint3.Helpers;

namespace Flint3.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private static Lazy<MainViewModel> _lazyVM = new Lazy<MainViewModel>(() => new MainViewModel());
        public static MainViewModel Instance => _lazyVM.Value;

        public Microsoft.UI.Dispatching.DispatcherQueue Dispatcher { get; set; }

        public SettingsService AppSettings { get; set; } = new SettingsService();

        public FlintLiteWindow LiteWindow = null;

        /// <summary>
        /// 控制主窗口根据当前的主题进行切换
        /// </summary>
        public Action ActSwitchAppTheme { get; set; } = null;

        /// <summary>
        /// 控制主窗口根据当前的设置更改背景材质
        /// </summary>
        public Action ActChangeBackdrop { get; set; } = null;

        /// <summary>
        /// 将焦点聚焦到搜索框
        /// </summary>
        public Action ActFocusOnTextBox { get; set; } = null;

        /// <summary>
        /// 将搜索框清空
        /// </summary>
        public Action ActClearTextBox { get; set; } = null;

        /// <summary>
        /// 将主窗口隐藏到系统托盘
        /// </summary>
        public Action ActHideWindow { get; set; } = null;

        /// <summary>
        /// 将主窗口保持置顶或取消置顶
        /// </summary>
        public Action<bool> ActPinWindow { get; set; } = null;

        /// <summary>
        /// 显示应用
        /// </summary>
        public Action ActShowWindow { get; set; } = null;

        /// <summary>
        /// 退出应用
        /// </summary>
        public Action ActExitWindow { get; set; } = null;

        /// <summary>
        /// 搜索展示结果
        /// </summary>
        public ObservableCollection<StarDictWordItem> SearchResultWordItems { get; private set; } = new ObservableCollection<StarDictWordItem>();

        public MainViewModel()
        {
            InitViewModel4Flint();
            InitViewModel4Glossary();
            InitViewModel4ShortcutKeys();
        }

        private void InitViewModel4Flint()
        {
            AppSettings.OnAppearanceSettingChanged += (index) => { ActSwitchAppTheme?.Invoke(); };
            AppSettings.OnBackdropSettingChanged += (index) => { ActChangeBackdrop?.Invoke(); };
            AppSettings.OnAcrylicOpacitySettingChanged += (opacity) => { };
        }

        /// <summary>
        /// 加载单词数据库
        /// </summary>
        public void LoadStarDict()
        {
            StarDictDataAccess.InitializeDatabase();
        }

        /// <summary>
        /// 查找模糊匹配的多个单词
        /// </summary>
        /// <param name="word"></param>
        public void MatchWord(string word, int limit = 10)
        {
            try
            {
                SearchResultWordItems.Clear();
                if (string.IsNullOrWhiteSpace(word)) return;

                word = System.Text.RegularExpressions.Regex.Replace(word, @"[^\w]*", "");

                var results = StarDictDataAccess.MatchWord(word, limit);
                if (results != null)
                {
                    foreach (StarDictWordItem item in results)
                    {
                        var wordItem = MakeupWord(item);
                        if (wordItem?.Word == word)
                        {
                            SearchResultWordItems.Insert(0, wordItem);
                        }
                        else
                        {
                            SearchResultWordItems.Add(wordItem);
                        }
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        private StarDictWordItem MakeupWord(StarDictWordItem item)
        {
            if (AppSettings.EnableEngDefinition == false)
            {
                item.Definition = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(item.Exchange))
            {
                string exchange = item.Exchange;
                string[] exchanges = exchange.Split('/');
                item.Exchanges = new List<WordExchangeItem>();
                string word = string.Empty;
                foreach (var exc in exchanges)
                {
                    if (exc.Contains("p:"))
                    {
                        word = exc.Replace("p:", "");
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            item.Exchanges.Add(new WordExchangeItem(word, "过去式"));
                        }
                    }
                    else if (exc.Contains("d:"))
                    {
                        word = exc.Replace("d:", "");
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            item.Exchanges.Add(new WordExchangeItem(word, "过去分词"));
                        }
                    }
                    else if (exc.Contains("i:"))
                    {
                        word = exc.Replace("i:", "");
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            item.Exchanges.Add(new WordExchangeItem(word, "现在分词"));
                        }
                    }
                    else if (exc.Contains("3:"))
                    {
                        word = exc.Replace("3:", "");
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            item.Exchanges.Add(new WordExchangeItem(word, "第三人称单数"));
                        }
                    }
                    else if (exc.Contains("r:"))
                    {
                        word = exc.Replace("r:", "");
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            item.Exchanges.Add(new WordExchangeItem(word, "比较级"));
                        }
                    }
                    else if (exc.Contains("t:"))
                    {
                        word = exc.Replace("t:", "");
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            item.Exchanges.Add(new WordExchangeItem(word, "最高级"));
                        }
                    }
                    else if (exc.Contains("s:"))
                    {
                        word = exc.Replace("s:", "");
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            item.Exchanges.Add(new WordExchangeItem(word, "复数"));
                        }
                    }
                    else if (exc.Contains("0:"))
                    {
                        word = exc.Replace("0:", "");
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            item.Exchanges.Add(new WordExchangeItem(word, "原型"));
                        }
                    }
                    //else if (exc.Contains("1:"))
                    //{
                    //    word = 
                    //    item.Exchanges.Add(new WordExchangeItem(exc.Replace("1:", ""), ""));
                    //}
                }

                if (item.Exchanges.Count <= 0)
                {
                    item.Exchanges = null;
                }
            }

            return item;
        }
    }
}
