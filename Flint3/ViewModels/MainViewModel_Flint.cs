using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Data;
using Flint3.Data.Models;

namespace Flint3.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [System.Text.RegularExpressions.GeneratedRegex(@"[^\w]*")]
        private static partial System.Text.RegularExpressions.Regex WordSearchRegex();

        private string _searchingWord = null;

        /// <summary>
        /// 当前输入的搜索词
        /// </summary>
        public string SearchingWord
        {
            get => _searchingWord;
            set
            {
                SetProperty(ref _searchingWord, value);

                MatchWord(_searchingWord);
            }
        }

        /// <summary>
        /// 查词结果
        /// </summary>
        public ObservableCollection<StarDictWordItem> SearchResultWordItems { get; private set; } = new ObservableCollection<StarDictWordItem>();

        /// <summary>
        /// 进行对单词搜索相关的初始化
        /// </summary>
        private void InitViewModel4Flint()
        {
            // 加载单词数据库（不使用await将无法捕获方法内部的异常）
            _ = StarDictDataAccess.InitializeDatabase();
        }

        /// <summary>
        /// 查找模糊匹配的多个单词
        /// </summary>
        /// <param name="word"></param>
        private async void MatchWord(string word, int limit = 10)
        {
            try
            {
                this.SearchResultWordItems.Clear();
                if (string.IsNullOrWhiteSpace(word)) return;

                word = WordSearchRegex().Replace(word, "");

                var results = await StarDictDataAccess.MatchWord(word, limit);
                if (results != null)
                {
                    foreach (StarDictWordItem item in results)
                    {
                        var wordItem = MakeupWordItem(item);
                        if (wordItem?.Word == word)
                        {
                            this.SearchResultWordItems.Insert(0, wordItem);
                        }
                        else
                        {
                            this.SearchResultWordItems.Add(wordItem);
                        }
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 将数据库返回的数据转换为界面展示的数据类型
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private StarDictWordItem MakeupWordItem(StarDictWordItem item)
        {
            try
            {
                if (this.AppSettings.EnableEngDefinition == false)
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
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                return item;
            }
        }
    }
}
