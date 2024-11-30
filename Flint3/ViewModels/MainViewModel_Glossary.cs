using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Data;
using Flint3.Data.Models;
using Flint3.Models;
using Windows.Storage;

namespace Flint3.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// 生词本生词列表滚动到顶部
        /// </summary>
        public Action ActScrollToGlossaryTop { get; set; } = null;

        /// <summary>
        /// 关闭添加到生词本的Popup
        /// </summary>
        public Action ActHideAddingPopup { get; set; } = null;

        /// <summary>
        /// 当前查看的生词本
        /// </summary>
        private GlossaryModelBase _selectedGlossary = null;
        public GlossaryModelBase SelectedGlossary
        {
            get => _selectedGlossary;
            set => SetProperty(ref _selectedGlossary, value);
        }

        /// <summary>
        /// 当前查看的生词
        /// </summary>
        private StarDictWordItem _selectedGlossaryWord = null;
        public StarDictWordItem SelectedGlossaryWord
        {
            get => _selectedGlossaryWord;
            set => SetProperty(ref _selectedGlossaryWord, value);
        }

        /// <summary>
        /// 当前查看的生词本单词列表
        /// </summary>
        public ObservableCollection<StarDictWordItem> GlossaryWordItems { get; private set; } = new ObservableCollection<StarDictWordItem>();

        #region 筛选生词列表

        /// <summary>
        /// 正在搜索的生词
        /// </summary>
        private string _filterGlossaryWord = "";
        public string FilterGlossaryWord
        {
            get => _filterGlossaryWord;
            set => SetProperty(ref _filterGlossaryWord, value);
        }

        /// <summary>
        /// 当前筛选的单词颜色
        /// </summary>
        private GlossaryColorsEnum _filterGlossaryColor = GlossaryColorsEnum.Transparent;
        public GlossaryColorsEnum FilterGlossaryColor
        {
            get => _filterGlossaryColor;
            set => SetProperty(ref _filterGlossaryColor, value);
        }

        /// <summary>
        /// 0-按照单词首字母排序，1-单词添加时间排序
        /// </summary>
        //private int _glossaryWordsOrderMode = 0;
        //public int GlossaryWordsOrderMode
        //{
        //    get => _glossaryWordsOrderMode;
        //    set => SetProperty(ref _glossaryWordsOrderMode, value);
        //}

        #endregion

        /// <summary>
        /// 进行对生词本相关的初始化
        /// </summary>
        private void InitViewModel4Glossary()
        {

        }

        /// <summary>
        /// 查看生词本
        /// </summary>
        /// <param name="selectedGlossary"></param>
        /// <param name="count"></param>
        public void SelectGlossary(GlossaryModelBase selectedGlossary, int count = 50)
        {
            Debug.WriteLine($"Select Glossary: {selectedGlossary.GlossaryTitle}");

            ActScrollToGlossaryTop?.Invoke();
            GlossaryWordItems.Clear();

            FilterGlossaryColor = GlossaryColorsEnum.Transparent;
            SelectedGlossary = selectedGlossary;
        }

        /// <summary>
        /// 清空正在显示的生词本单词列表
        /// </summary>
        /// <param name="count"></param>
        /// <param name="word"></param>
        /// <param name="color"></param>
        public void ClearGlossaryWords()
        {
            ActScrollToGlossaryTop?.Invoke();
            GlossaryWordItems.Clear();
        }

        /// <summary>
        /// 增量加载生词
        /// </summary>
        public void GetMoreGlossaryWords(int count = 50)
        {
            try
            {
                Debug.WriteLine($"Getting More GlossaryWords: {SelectedGlossary.GlossaryTitle}, {FilterGlossaryWord}, {FilterGlossaryColor}");

                if (SelectedGlossary is Models.GlossaryExModel)
                {
                    long lastId = GlossaryWordItems.Count > 0 ? GlossaryWordItems.Last().Id : -1;
                    GetMoreExGlossaryWords(lastId, count, FilterGlossaryWord.Trim());
                }
                else if (SelectedGlossary is GlossaryMyModel)
                {
                    long lastId = GlossaryWordItems.Count > 0 ? GlossaryWordItems.Last().Id : long.MaxValue;
                    GetMoreMyGlossaryWords(lastId, count, FilterGlossaryWord.Trim(), FilterGlossaryColor/*, GlossaryWordsOrderMode == 0*/);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"GetMoreGlossaryWords Error: {e.Message}");
            }
        }

        #region 内置生词本

        /// <summary>
        /// 内置生词本列表
        /// </summary>
        public ObservableCollection<GlossaryExModel> ExGlossaries { get; private set; } = new ObservableCollection<GlossaryExModel>();

        /// <summary>
        /// 初始化内置生词本
        /// </summary>
        public void LoadExGlossaries()
        {
            try
            {
                ExGlossaries?.Clear();

                ExGlossaries.Add(new()
                {
                    GlossaryTitle = "牛津核心词汇",
                    ExtraGlossaryInternalTag = "oxford",
                    GlossaryIcon = "\uE128",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 3461
                });

                ExGlossaries.Add(new()
                {
                    GlossaryTitle = "雅思词汇",
                    ExtraGlossaryInternalTag = "ielts",
                    GlossaryIcon = "\uF7DB",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 5040
                });

                ExGlossaries.Add(new()
                {
                    GlossaryTitle = "托福词汇",
                    ExtraGlossaryInternalTag = "toefl",
                    GlossaryIcon = "\uF7DB",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 6974
                });

                ExGlossaries.Add(new()
                {
                    GlossaryTitle = "GRE 词汇",
                    ExtraGlossaryInternalTag = "gre",
                    GlossaryIcon = "\uF7DB",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 7504
                });

                ExGlossaries.Add(new()
                {
                    GlossaryTitle = "考研词汇",
                    ExtraGlossaryInternalTag = "ky",
                    GlossaryIcon = "\uE7BE",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 4801
                });

                ExGlossaries.Add(new()
                {
                    GlossaryTitle = "CET 6 词汇",
                    ExtraGlossaryInternalTag = "cet6",
                    GlossaryIcon = "\uE1D3",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 5407
                });

                ExGlossaries.Add(new()
                {
                    GlossaryTitle = "CET 4 词汇",
                    ExtraGlossaryInternalTag = "cet4",
                    GlossaryIcon = "\uE1D3",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 3849
                });

                ExGlossaries.Add(new()
                {
                    GlossaryTitle = "高考词汇",
                    ExtraGlossaryInternalTag = "gk",
                    GlossaryIcon = "\uE7BC",
                    IsReadOnly = true,
                    GlossaryDescription = "这是一个扩展生词本，词汇仅供参考，请勿仅依赖这些词汇进行复习。",
                    GlossaryWordsCount = 3677
                });

                ExGlossaries.Add(new()
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
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 增量加载内置生词本的单词
        /// </summary>
        /// <param name="count"></param>
        private void GetMoreExGlossaryWords(long lastId, int count, string word)
        {
            Debug.WriteLine($"Last id: {lastId}, count={GlossaryWordItems.Count}");

            if (SelectedGlossary is GlossaryExModel glossary)
            {
                var list = StarDictDataAccess.GetExtraGlossaryWords(glossary.ExtraGlossaryInternalTag, lastId, count, word);

                foreach (var item in list)
                {
                    GlossaryWordItems.Add(MakeupWord(item));
                }
            }
        }

        #endregion

        #region 我的生词本

        /// <summary>
        /// 当前是否正在编辑生词本属性
        /// </summary>
        private bool _editingGlossaryProperty = false;
        public bool EditingGlossaryProperty
        {
            get => _editingGlossaryProperty;
            set => SetProperty(ref _editingGlossaryProperty, value);
        }

        /// <summary>
        /// 用户生词本列表
        /// </summary>
        public ObservableCollection<GlossaryMyModel> MyGlossaries { get; private set; } = new ObservableCollection<GlossaryMyModel>();

        /// <summary>
        /// 加载我的生词本
        /// </summary>
        public void LoadMyGlossaries()
        {
            try
            {
                MyGlossaries?.Clear();

                StorageFolder documentsFolder = StorageFolder.GetFolderFromPathAsync(UserDataPaths.GetDefault().Documents).GetAwaiter().GetResult();

                var oldFlintFolder = documentsFolder.TryGetItemAsync("Flint").GetAwaiter().GetResult();

                var noMewingFolder = documentsFolder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();

                // 把Flint文件夹迁移到NoMewing下面
                if (oldFlintFolder != null && !Directory.Exists(Path.Combine(noMewingFolder.Path, "Flint")))
                {
                    Directory.Move(oldFlintFolder.Path, Path.Combine(noMewingFolder.Path, "Flint"));
                }

                var flintFolder = noMewingFolder.CreateFolderAsync("Flint", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();

                GlossaryDataAccess.CloseDatabase();
                GlossaryDataAccess.LoadDatabase(flintFolder);

                GlossaryDataAccess.GetAllGlossaries().ForEach(item =>
                    MyGlossaries.Add(new GlossaryMyModel()
                    {
                        Id = item.Id,
                        GlossaryTitle = item.Title,
                        GlossaryDescription = item.Description,
                        GlossaryIcon = "\uEE56"
                    }));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 增量加载内置生词本的单词
        /// </summary>
        /// <param name="count"></param>
        private void GetMoreMyGlossaryWords(long lastId, int count, string word, GlossaryColorsEnum color/*, bool orderByWord*/)
        {
            Debug.WriteLine($"Last id: {lastId}, count={GlossaryWordItems.Count}, word={word}, color={color}");

            if (SelectedGlossary is GlossaryMyModel glossary)
            {
                var list = GlossaryDataAccess.GetGlossaryWords(glossary.Id, lastId, count, word, color/*, orderByWord*/);

                foreach (var item in list)
                {
                    GlossaryWordItems.Add(MakeupWord(item));
                }
            }
        }

        /// <summary>
        /// 创建新的生词本
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        public void CreateMyGlossary(string name, string desc)
        {
            try
            {
                GlossaryDataAccess.AddOneGlossary(name, desc);

                LoadMyGlossaries();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 编辑生词本
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        public void UpdateMyGlossary(int id, string name, string desc)
        {
            try
            {
                GlossaryDataAccess.UpdateOneGlossary(id, name, desc);

                LoadMyGlossaries();

                if (SelectedGlossary.Id == id)
                {
                    SelectedGlossary.GlossaryTitle = name;
                    SelectedGlossary.GlossaryDescription = desc;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 删除生词本
        /// </summary>
        /// <param name="id"></param>
        public void DeleteMyGlossary(int id)
        {
            try
            {
                GlossaryDataAccess.DeleteOneGlossary(id);

                LoadMyGlossaries();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
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
        public void AddWordToMyGlossary(long wordid, int glossaryId, string word, string phonetic, string definition, string translation, string exchange, string description, GlossaryColorsEnum color)
        {
            try
            {
                GlossaryDataAccess.AddGlossaryWord(wordid, glossaryId, word, phonetic, definition, translation, exchange, description, color);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 修改指定id的生词
        /// </summary>
        /// <param name="wordid"></param>
        /// <param name="desc"></param>
        /// <param name="color"></param>
        public void UpdateWordFromMyGlossary(long id, string desc, GlossaryColorsEnum color)
        {
            try
            {
                GlossaryDataAccess.UpdateGlossaryWord(id, desc, color);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 删除指定id的生词
        /// </summary>
        /// <param name="id"></param>
        public void DeleteWordFromMyGlossary(long id)
        {
            try
            {
                GlossaryDataAccess.DeleteGlossaryWord(id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 获取指定ID的生词本内生词数量
        /// </summary>
        /// <param name="glossaryId"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public int GetWordsCountOfMyGlossary(int glossaryId, GlossaryColorsEnum color)
        {
            try
            {
                return GlossaryDataAccess.GetGlossaryWordsCount(glossaryId, color);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return 0;
        }

        #endregion

        #region 添加到我的生词本

        /// <summary>
        /// 可以添加当前单词的生词本列表
        /// </summary>
        public ObservableCollection<GlossaryMyModel> AddingGlossaries { get; private set; } = new ObservableCollection<GlossaryMyModel>();

        /// <summary>
        /// 正在添加的单词
        /// </summary>
        private StarDictWordItem _addingWordItem = null;
        public StarDictWordItem AddingWordItem
        {
            get => _addingWordItem;
            set => SetProperty(ref _addingWordItem, value);
        }

        /// <summary>
        /// 当前添加的单词颜色
        /// </summary>
        private GlossaryColorsEnum _addingWordColor = GlossaryColorsEnum.Transparent;
        public GlossaryColorsEnum AddingWordColor
        {
            get => _addingWordColor;
            set => SetProperty(ref _addingWordColor, value);
        }

        /// <summary>
        /// 是否正在检索当前添加单词在各个生词本中是否存在
        /// </summary>
        private bool _searchingWordItemExist = false;
        public bool SearchingWordItemExist
        {
            get => _searchingWordItemExist;
            set => SetProperty(ref _searchingWordItemExist, value);
        }

        /// <summary>
        /// 获取可以添加当前单词的生词本列表
        /// </summary>
        public async void GetAddGlossariesList()
        {
            SearchingWordItemExist = true;

            AddingGlossaries.Clear();
            ObservableCollection<GlossaryMyModel> tempList = new ObservableCollection<GlossaryMyModel>();

            await Task.Run(() =>
            {
                foreach (var item in MyGlossaries)
                {
                    if (!GlossaryDataAccess.IfExistGlossaryWord(AddingWordItem.Id, item.Id))
                    {
                        tempList.Add(item);
                    }
                }
            });

            foreach (var item in tempList)
            {
                AddingGlossaries.Add(item);
            }

            SearchingWordItemExist = false;
        }

        #endregion

    }
}
