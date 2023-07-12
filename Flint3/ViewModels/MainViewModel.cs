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

namespace Flint3.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private static Lazy<MainViewModel> _lazyVM = new Lazy<MainViewModel>(() => new MainViewModel());
        public static MainViewModel Instance => _lazyVM.Value;

        public SettingsService AppSettings { get; set; } = new SettingsService();

        public Action ActSwitchAppTheme { get; set; } = null;
        public Action ActClearTextBoxes { get; set; } = null;

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ObservableCollection<StarDictWordItem> SearchResultWordItems { get; private set; } = new ObservableCollection<StarDictWordItem>();

        public MainViewModel() { }

        public void QueryWord(string word)
        {
            try
            {
                SearchResultWordItems.Clear();
                if (string.IsNullOrWhiteSpace(word)) return;

                var results = StarDictDataAccess.QueryWord(word);
                if (results != null)
                {
                    foreach (var item in results)
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
                            foreach (var exc in exchanges)
                            {
                                if (exc.Contains("p:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("p:", ""), "过去式"));
                                }
                                else if (exc.Contains("d:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("d:", ""), "过去分词"));
                                }
                                else if (exc.Contains("i:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("i:", ""), "现在分词"));
                                }
                                else if (exc.Contains("3:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("3:", ""), "第三人称单数"));
                                }
                                else if (exc.Contains("r:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("r:", ""), "比较级"));
                                }
                                else if (exc.Contains("t:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("t:", ""), "最高级"));
                                }
                                else if (exc.Contains("s:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("s:", ""), "复数"));
                                }
                                else if (exc.Contains("0:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("0:", ""), "原型"));
                                }
                                //else if (exc.Contains("1:"))
                                //{
                                //    item.Exchanges.Add(new WordExchangeItem(exc.Replace("1:", ""), "[]"));
                                //}
                            }
                            if (item.Exchanges.Count <= 0) { item.Exchanges = null; }
                        }

                        SearchResultWordItems.Add(item);
                    }
                }
            }
            catch { }
        }

        public void MatchWord(string word)
        {
            try
            {
                SearchResultWordItems.Clear();
                if (string.IsNullOrWhiteSpace(word)) return;

                word = System.Text.RegularExpressions.Regex.Replace(word, @"[^\w]*", "");

                var results = StarDictDataAccess.MatchWord(word);
                if (results != null)
                {
                    foreach (var item in results)
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
                            foreach (var exc in exchanges)
                            {
                                if (exc.Contains("p:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("p:", ""), "过去式"));
                                }
                                else if (exc.Contains("d:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("d:", ""), "过去分词"));
                                }
                                else if (exc.Contains("i:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("i:", ""), "现在分词"));
                                }
                                else if (exc.Contains("3:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("3:", ""), "第三人称单数"));
                                }
                                else if (exc.Contains("r:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("r:", ""), "比较级"));
                                }
                                else if (exc.Contains("t:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("t:", ""), "最高级"));
                                }
                                else if (exc.Contains("s:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("s:", ""), "复数"));
                                }
                                else if (exc.Contains("0:"))
                                {
                                    item.Exchanges.Add(new WordExchangeItem(exc.Replace("0:", ""), "原型"));
                                }
                                //else if (exc.Contains("1:"))
                                //{
                                //    item.Exchanges.Add(new WordExchangeItem(exc.Replace("1:", ""), ""));
                                //}
                            }
                            if (item.Exchanges.Count <= 0) { item.Exchanges = null; }
                        }

                        SearchResultWordItems.Add(item);
                    }
                }
            }
            catch { }
        }
    }
}
