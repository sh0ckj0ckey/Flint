using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Data;
using Flint3.Data.Models;
using Flint3.Models;

namespace Flint3.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // 生词本单词列表的查询统一受其控制，新的查询触发时要先取消旧的未完成的查询
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private GlossaryBaseModel _selectedGlossary = null;

        private StarDictWordItem _selectedGlossaryWord = null;

        private bool _loadedGlossaryWords = true;

        /// <summary>
        /// 当前查看的生词本
        /// </summary>
        public GlossaryBaseModel SelectedGlossary
        {
            get => _selectedGlossary;
            private set => SetProperty(ref _selectedGlossary, value);
        }

        /// <summary>
        /// 当前查看的生词
        /// </summary>
        public StarDictWordItem SelectedGlossaryWord
        {
            get => _selectedGlossaryWord;
            private set => SetProperty(ref _selectedGlossaryWord, value);
        }

        /// <summary>
        /// 当前查看的生词本单词列表
        /// </summary>
        public ObservableCollection<StarDictWordItem> GlossaryWordItems = new ObservableCollection<StarDictWordItem>();

        /// <summary>
        /// 生词是否已经加载结束
        /// </summary>
        public bool LoadedGlossaryWords
        {
            get => _loadedGlossaryWords;
            private set => SetProperty(ref _loadedGlossaryWords, value);
        }

        #region 筛选生词列表

        private string _filterGlossaryWord = "";

        private GlossaryColorsEnum _filterGlossaryColor = GlossaryColorsEnum.Transparent;

        private int _glossaryWordsOrderMode = 0;

        /// <summary>
        /// 正在搜索的生词
        /// </summary>
        public string FilterGlossaryWord
        {
            get => _filterGlossaryWord;
            set => SetProperty(ref _filterGlossaryWord, value);
        }

        /// <summary>
        /// 当前筛选的单词颜色
        /// </summary>
        public GlossaryColorsEnum FilterGlossaryColor
        {
            get => _filterGlossaryColor;
            set => SetProperty(ref _filterGlossaryColor, value);
        }

        /// <summary>
        /// 0-按照单词首字母排序，1-单词添加时间排序
        /// </summary>
        public int GlossaryWordsOrderMode
        {
            get => _glossaryWordsOrderMode;
            set => SetProperty(ref _glossaryWordsOrderMode, value);
        }

        #endregion

        /// <summary>
        /// 进行对生词本相关的初始化
        /// </summary>
        private void InitViewModel4Glossary()
        {
            // 加载生词本数据库
            this.LoadMyGlossaries();
            this.LoadExGlossaries();
        }

        /// <summary>
        /// 查看生词本
        /// </summary>
        /// <param name="selectedGlossary"></param>
        public void SelectGlossary(GlossaryBaseModel selectedGlossary)
        {
            System.Diagnostics.Trace.WriteLine($"Select Glossary: {selectedGlossary.GlossaryTitle}");
            this.SelectedGlossary = selectedGlossary;
            this.SelectedGlossaryWord = null;
            this.GlossaryWordItems.Clear();
            this.LoadedGlossaryWords = true;
            this.FilterGlossaryWord = "";
            this.FilterGlossaryColor = GlossaryColorsEnum.Transparent;
        }

        /// <summary>
        /// 查看生词
        /// </summary>
        /// <param name="wordItem"></param>
        public void SelectGlossaryWord(StarDictWordItem wordItem)
        {
            System.Diagnostics.Trace.WriteLine($"Select Glossary Word: {wordItem.Word}");
            this.SelectedGlossaryWord = wordItem;
        }

        /// <summary>
        /// 加载所有生词
        /// </summary>
        /// <returns></returns>
        public async Task GetAllGlossaryWords()
        {
            // 取消前一次的查询
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            try
            {
                System.Diagnostics.Trace.WriteLine($"Getting All GlossaryWords: {this.SelectedGlossary.GlossaryTitle}, {this.FilterGlossaryWord}, {this.FilterGlossaryColor}, {this.GlossaryWordsOrderMode}");

                this.LoadedGlossaryWords = false;
                this.GlossaryWordItems.Clear();

                if (this.SelectedGlossary is Models.GlossaryExModel)
                {
                    await GetAllExGlossaryWords(this.FilterGlossaryWord.Trim(), token);
                }
                else if (this.SelectedGlossary is GlossaryMyModel)
                {
                    await GetAllMyGlossaryWords(this.FilterGlossaryWord.Trim(), this.FilterGlossaryColor, this.GlossaryWordsOrderMode != 1, token);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine($"GetMoreGlossaryWords Error: {e.Message}");
            }
            finally
            {
                // 如果并非主动取消，则改为加载完成，否则保持加载中状态，因为在主动取消后一定会再跟一次新的查询
                if (!token.IsCancellationRequested)
                {
                    this.LoadedGlossaryWords = true;
                }
            }
        }

        /// <summary>
        /// 增量加载生词
        /// </summary>
        public async Task GetMoreGlossaryWords(int count = 50)
        {
            // 取消前一次的查询
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            try
            {
                System.Diagnostics.Trace.WriteLine($"Getting More GlossaryWords: {this.SelectedGlossary.GlossaryTitle}, {this.FilterGlossaryWord}, {this.FilterGlossaryColor}");

                this.LoadedGlossaryWords = false;

                if (this.SelectedGlossary is Models.GlossaryExModel)
                {
                    // 内置生词本单词为顺序，因此从-1开始顺序加载
                    long lastId = this.GlossaryWordItems.Count > 0 ? this.GlossaryWordItems.Last().Id : -1;
                    await GetMoreExGlossaryWords(lastId, count, this.FilterGlossaryWord.Trim(), token);
                }
                else if (this.SelectedGlossary is GlossaryMyModel)
                {
                    // 用户生词本为倒序，新添加的在后面，因此从无穷大开始倒叙加载
                    long lastId = this.GlossaryWordItems.Count > 0 ? this.GlossaryWordItems.Last().Id : long.MaxValue;
                    await GetMoreMyGlossaryWords(lastId, count, this.FilterGlossaryWord.Trim(), this.FilterGlossaryColor, token);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine($"GetMoreGlossaryWords Error: {e.Message}");
            }
            finally
            {
                // 如果并非主动取消，则改为加载完成，否则保持加载中状态，因为在主动取消后一定会再跟一次新的查询
                if (!token.IsCancellationRequested)
                {
                    this.LoadedGlossaryWords = true;
                }
            }
        }

        #region 内置生词本

        /// <summary>
        /// 内置生词本列表
        /// </summary>
        public ObservableCollection<GlossaryExModel> ExGlossaries { get; } = new ObservableCollection<GlossaryExModel>();

        /// <summary>
        /// 初始化内置生词本
        /// </summary>
        public void LoadExGlossaries()
        {
            try
            {
                this.ExGlossaries.Clear();

                this.ExGlossaries.Add(new()
                {
                    GlossaryTitle = "牛津核心词汇",
                    ExtraGlossaryInternalTag = "oxford",
                    GlossaryIcon = "\uE128",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 3461
                });

                this.ExGlossaries.Add(new()
                {
                    GlossaryTitle = "雅思词汇",
                    ExtraGlossaryInternalTag = "ielts",
                    GlossaryIcon = "\uF7DB",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 5040
                });

                this.ExGlossaries.Add(new()
                {
                    GlossaryTitle = "托福词汇",
                    ExtraGlossaryInternalTag = "toefl",
                    GlossaryIcon = "\uF7DB",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 6974
                });

                this.ExGlossaries.Add(new()
                {
                    GlossaryTitle = "GRE 词汇",
                    ExtraGlossaryInternalTag = "gre",
                    GlossaryIcon = "\uF7DB",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 7504
                });

                this.ExGlossaries.Add(new()
                {
                    GlossaryTitle = "考研词汇",
                    ExtraGlossaryInternalTag = "ky",
                    GlossaryIcon = "\uE7BE",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 4801
                });

                this.ExGlossaries.Add(new()
                {
                    GlossaryTitle = "CET 6 词汇",
                    ExtraGlossaryInternalTag = "cet6",
                    GlossaryIcon = "\uE1D3",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 5407
                });

                this.ExGlossaries.Add(new()
                {
                    GlossaryTitle = "CET 4 词汇",
                    ExtraGlossaryInternalTag = "cet4",
                    GlossaryIcon = "\uE1D3",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 3849
                });

                this.ExGlossaries.Add(new()
                {
                    GlossaryTitle = "高考词汇",
                    ExtraGlossaryInternalTag = "gk",
                    GlossaryIcon = "\uE7BC",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 3677
                });

                this.ExGlossaries.Add(new()
                {
                    GlossaryTitle = "中考词汇",
                    ExtraGlossaryInternalTag = "zk",
                    GlossaryIcon = "\uE913",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 1603
                });

                //await Task.Run(() =>
                //{
                //    foreach (var item in ExGlossaries)
                //    {
                //        // 查找每个生词本的单词数量，并且生成描述
                //        var count = StarDictDataAccess.GetExGlossaryWordCount(item.ExtraGlossaryInternalTag);
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
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 加载内置生词本的所有单词
        /// </summary>
        /// <param name="count"></param>
        private async Task GetAllExGlossaryWords(string word, CancellationToken token)
        {
            await Task.Run(async () =>
            {
                if (this.SelectedGlossary is GlossaryExModel glossary)
                {
                    var list = await StarDictDataAccess.GetAllExtraGlossaryWords(glossary.ExtraGlossaryInternalTag, word, token);
                    List<StarDictWordItem> words = new List<StarDictWordItem>();
                    list?.ForEach(item => words.Add(MakeupWordItem(item)));

                    this.Dispatcher.TryEnqueue(() =>
                    {
                        this.GlossaryWordItems.Clear();
                        words.ForEach(item => this.GlossaryWordItems.Add(item));
                    });
                }
            }, token);
        }

        /// <summary>
        /// 增量加载内置生词本的单词
        /// </summary>
        /// <param name="count"></param>
        private async Task GetMoreExGlossaryWords(long lastId, int count, string word, CancellationToken token)
        {
            System.Diagnostics.Trace.WriteLine($"Last id: {lastId}, count={GlossaryWordItems.Count}");

            if (this.SelectedGlossary is GlossaryExModel glossary)
            {
                var list = await StarDictDataAccess.GetIncrementalExtraGlossaryWords(glossary.ExtraGlossaryInternalTag, lastId, count, word, token);
                List<StarDictWordItem> words = new List<StarDictWordItem>();
                list?.ForEach(item => words.Add(MakeupWordItem(item)));
                words.ForEach(item => this.GlossaryWordItems.Add(item));
            }
        }

        #endregion

        #region 我的生词本

        /// <summary>
        /// 用户生词本列表
        /// </summary>
        public ObservableCollection<GlossaryMyModel> MyGlossaries { get; } = new ObservableCollection<GlossaryMyModel>();

        /// <summary>
        /// 加载我的生词本
        /// </summary>
        public async void LoadMyGlossaries()
        {
            try
            {
                GlossaryDataAccess.CloseDatabase();
                await GlossaryDataAccess.LoadDatabase();

                var glossaries = await GlossaryDataAccess.GetGlossaries();

                this.MyGlossaries?.Clear();
                glossaries.ForEach(item => this.MyGlossaries.Add(new GlossaryMyModel()
                {
                    Id = item.Id,
                    GlossaryTitle = item.Title,
                    GlossaryDescription = item.Description,
                    GlossaryIcon = "\uEE56"
                }));
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 获取生词本内的所有单词
        /// </summary>
        /// <param name="word"></param>
        /// <param name="color"></param>
        /// <param name="orderByWord"></param>
        /// <returns></returns>
        private async Task GetAllMyGlossaryWords(string word, GlossaryColorsEnum color, bool orderByWord, CancellationToken token)
        {
            await Task.Run(async () =>
            {
                if (this.SelectedGlossary is GlossaryMyModel glossary)
                {
                    var list = await GlossaryDataAccess.GetAllGlossaryWords(glossary.Id, word, color, orderByWord, token);
                    List<StarDictWordItem> words = new List<StarDictWordItem>();
                    list?.ForEach(item => words.Add(MakeupWordItem(item)));

                    this.Dispatcher.TryEnqueue(() =>
                    {
                        this.GlossaryWordItems.Clear();
                        words.ForEach(item => this.GlossaryWordItems.Add(item));
                    });
                }
            }, token);
        }

        /// <summary>
        /// 增量加载生词本的单词
        /// </summary>
        /// <param name="lastId"></param>
        /// <param name="count"></param>
        /// <param name="word"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private async Task GetMoreMyGlossaryWords(long lastId, int count, string word, GlossaryColorsEnum color, CancellationToken token)
        {
            System.Diagnostics.Trace.WriteLine($"Last id: {lastId}, count={GlossaryWordItems.Count}, word={word}, color={color}");

            if (this.SelectedGlossary is GlossaryMyModel glossary)
            {
                var list = await GlossaryDataAccess.GetIncrementalGlossaryWords(glossary.Id, lastId, count, word, color, token);
                List<StarDictWordItem> words = new List<StarDictWordItem>();
                list?.ForEach(item => words.Add(MakeupWordItem(item)));
                words.ForEach(item => this.GlossaryWordItems.Add(item));
            }
        }

        /// <summary>
        /// 创建新的生词本
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        public async Task AddMyGlossary(string name, string desc)
        {
            try
            {
                await GlossaryDataAccess.AddGlossary(name, desc);
                LoadMyGlossaries();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 编辑生词本
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        public async Task UpdateMyGlossary(int id, string name, string desc)
        {
            try
            {
                await GlossaryDataAccess.UpdateGlossary(id, name, desc);
                LoadMyGlossaries();

                if (this.SelectedGlossary.Id == id)
                {
                    this.SelectedGlossary.GlossaryTitle = name;
                    this.SelectedGlossary.GlossaryDescription = desc;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 删除生词本
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteMyGlossary(int id)
        {
            try
            {
                await GlossaryDataAccess.DeleteGlossary(id);
                LoadMyGlossaries();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 将一个词添加到我的生词本
        /// </summary>
        /// <param name="wordid"></param>
        /// <param name="glossaryId"></param>
        /// <param name="word"></param>
        /// <param name="phonetic"></param>
        /// <param name="definition"></param>
        /// <param name="translation"></param>
        /// <param name="exchange"></param>
        /// <param name="description"></param>
        /// <param name="color"></param>
        public async Task AddWordToMyGlossary(long wordid, int glossaryId, string word, string phonetic, string definition, string translation, string exchange, string description, GlossaryColorsEnum color)
        {
            try
            {
                await GlossaryDataAccess.AddGlossaryWord(wordid, glossaryId, word, phonetic, definition, translation, exchange, description, color);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 修改指定id的生词
        /// </summary>
        /// <param name="wordid"></param>
        /// <param name="desc"></param>
        /// <param name="color"></param>
        public async Task UpdateWordFromMyGlossary(long id, string desc, GlossaryColorsEnum color)
        {
            try
            {
                await GlossaryDataAccess.UpdateGlossaryWord(id, desc, color);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 删除指定id的生词
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteWordFromMyGlossary(long id)
        {
            try
            {
                await GlossaryDataAccess.DeleteGlossaryWord(id);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 获取指定ID的生词本内生词数量
        /// </summary>
        /// <param name="glossaryId"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public async Task<int> GetWordsCountOfMyGlossary(int glossaryId, GlossaryColorsEnum color)
        {
            try
            {
                return await GlossaryDataAccess.GetGlossaryWordsCount(glossaryId, color);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
            return 0;
        }

        /// <summary>
        /// 返回所有不包含指定单词的生词本
        /// </summary>
        /// <param name="addingWordItem"></param>
        /// <returns></returns>
        public async Task<List<GlossaryMyModel>> GetGlossariesWithoutThisWord(StarDictWordItem addingWordItem)
        {
            List<GlossaryMyModel> result = new List<GlossaryMyModel>();

            try
            {
                foreach (var item in this.MyGlossaries)
                {
                    if (!await GlossaryDataAccess.CheckGlossaryContainsWord(addingWordItem.Id, item.Id))
                    {
                        result.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }

            return result;
        }

        #endregion

    }
}
