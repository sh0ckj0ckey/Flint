using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flint3.Data.Models;
using Microsoft.Data.Sqlite;
using Windows.Storage;

namespace Flint3.Data
{
    public static class GlossaryDataAccess
    {
        private static SqliteConnection _glossaryDb = null;

        public static async Task LoadDatabase()
        {
            StorageFolder documentsFolder = await StorageFolder.GetFolderFromPathAsync(UserDataPaths.GetDefault().Documents);
            var noMewingFolder = await documentsFolder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
            var flintFolder = await noMewingFolder.CreateFolderAsync("Flint", CreationCollisionOption.OpenIfExists);
            var file = await flintFolder.CreateFileAsync("flint_glossary.db", CreationCollisionOption.OpenIfExists);
            string dbpath = file.Path;
            _glossaryDb = new SqliteConnection($"Filename={dbpath}");
            await _glossaryDb.OpenAsync();

            string categoryTableCommand =
                "CREATE TABLE IF NOT EXISTS glossaryCategory (" +
                    "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                    "title TEXT," +
                    "description TEXT);";
            using SqliteCommand createCategoryTable = new SqliteCommand(categoryTableCommand, _glossaryDb);
            await createCategoryTable.ExecuteNonQueryAsync();

            string glossaryTableCommand =
                "CREATE TABLE IF NOT EXISTS glossary (" +
                    "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                    "wordid INTEGER," +
                    "glossaryid INTEGER NOT NULL," +
                    "word VARCHAR(64) COLLATE NOCASE NOT NULL," +
                    "phonetic VARCHAR(64)," +
                    "definition TEXT," +
                    "translation TEXT," +
                    "exchange TEXT," +
                    "description TEXT," +
                    "color INTEGER);";
            using SqliteCommand createGlossaryTable = new SqliteCommand(glossaryTableCommand, _glossaryDb);
            await createGlossaryTable.ExecuteNonQueryAsync();
        }

        public static void CloseDatabase()
        {
            try
            {
                _glossaryDb?.Close();
                _glossaryDb?.Dispose();
                _glossaryDb = null;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        #region 生词本

        /// <summary>
        /// 获取所有的生词本列表
        /// </summary>
        /// <returns></returns>
        public static async Task<List<GlossaryItem>> GetGlossaries()
        {
            try
            {
                List<GlossaryItem> results = new List<GlossaryItem>();
                using SqliteCommand selectCommand = new SqliteCommand($"SELECT * FROM glossaryCategory", _glossaryDb);
                using SqliteDataReader query = await selectCommand.ExecuteReaderAsync();
                while (await query.ReadAsync() == true)
                {
                    GlossaryItem item = new GlossaryItem();
                    item.Id = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    item.Title = query.IsDBNull(1) ? string.Empty : query.GetString(1);
                    item.Description = query.IsDBNull(2) ? string.Empty : query.GetString(2);
                    results.Add(item);
                }

                return results;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return null;
        }

        /// <summary>
        /// 添加一个生词本
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static async Task AddGlossary(string title, string desc)
        {
            try
            {
                using SqliteCommand insertCommand = new SqliteCommand($"INSERT INTO glossaryCategory(title, description) VALUES($title, $desc);", _glossaryDb);
                insertCommand.Parameters.AddWithValue("$title", title);
                insertCommand.Parameters.AddWithValue("$desc", desc);
                await insertCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 修改生词本的属性
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static async Task UpdateGlossary(int id, string title, string desc)
        {
            try
            {
                using SqliteCommand updateCommand = new SqliteCommand($"UPDATE glossaryCategory SET title=$title, description=$desc WHERE id=$glossaryid;", _glossaryDb);
                updateCommand.Parameters.AddWithValue("$title", title);
                updateCommand.Parameters.AddWithValue("$desc", desc);
                updateCommand.Parameters.AddWithValue("$glossaryid", id);
                await updateCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 删除一个生词本，并删除glossary表中所有关联的生词
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static async Task DeleteGlossary(int id)
        {
            try
            {
                using SqliteCommand deleteCommand = new SqliteCommand($"DELETE FROM glossaryCategory WHERE id=$glossaryid;", _glossaryDb);
                deleteCommand.Parameters.AddWithValue("$glossaryid", id);
                await deleteCommand.ExecuteNonQueryAsync();

                using SqliteCommand deleteWordsCommand = new SqliteCommand($"DELETE FROM glossary WHERE glossaryid=$glossaryid;", _glossaryDb);
                deleteWordsCommand.Parameters.AddWithValue("$glossaryid", id);
                await deleteWordsCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 获取指定ID的生词本内生词个数
        /// </summary>
        /// <param name="glossaryId"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static async Task<int> GetGlossaryWordsCount(int glossaryId, GlossaryColorsEnum color)
        {
            try
            {
                using SqliteCommand selectCommand = color != GlossaryColorsEnum.Transparent
                    ? new SqliteCommand($"select count(*) from glossary where glossaryid=$glossaryid AND color=$color", _glossaryDb)
                    : new SqliteCommand($"select count(*) from glossary where glossaryid=$glossaryid", _glossaryDb);

                selectCommand.Parameters.AddWithValue("$glossaryid", glossaryId);
                if (color != GlossaryColorsEnum.Transparent)
                {
                    selectCommand.Parameters.AddWithValue("$color", (int)color);
                }

                using SqliteDataReader query = await selectCommand.ExecuteReaderAsync();
                while (await query.ReadAsync() == true)
                {
                    StarDictWordItem item = new StarDictWordItem();
                    var count = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    return count;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return -1;
        }

        #endregion

        #region 生词

        /// <summary>
        /// 获取某个生词本的全部生词，按照添加时间或者首字母排序
        /// </summary>
        /// <returns></returns>
        public static async Task<List<StarDictWordItem>> GetAllGlossaryWords(int glossaryId, string word, GlossaryColorsEnum color, bool orderByWord, CancellationToken cancellationToken)
        {
            try
            {
                List<StarDictWordItem> results = new List<StarDictWordItem>();

                string sql = $"SELECT * FROM glossary WHERE glossaryid=$glossaryid";

                if (!string.IsNullOrWhiteSpace(word))
                {
                    sql += $" AND word LIKE $word";
                }

                if (color != GlossaryColorsEnum.Transparent)
                {
                    sql += $" AND color=$color";
                }

                sql += orderByWord ? $" ORDER BY word,id COLLATE NOCASE" : $" ORDER BY id DESC";

                using SqliteCommand selectCommand = new SqliteCommand(sql, _glossaryDb);

                selectCommand.Parameters.AddWithValue("$glossaryid", glossaryId);

                if (!string.IsNullOrWhiteSpace(word))
                {
                    selectCommand.Parameters.AddWithValue("$word", word + "%");
                }

                if (color != GlossaryColorsEnum.Transparent)
                {
                    selectCommand.Parameters.AddWithValue("$color", (int)color);
                }

                using SqliteDataReader query = await selectCommand.ExecuteReaderAsync(cancellationToken);
                while (await query.ReadAsync(cancellationToken) == true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    StarDictWordItem item = new StarDictWordItem();
                    item.Id = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    item.Word = query.IsDBNull(3) ? string.Empty : query.GetString(3);
                    item.Phonetic = query.IsDBNull(4) ? string.Empty : query.GetString(4);
                    item.Definition = query.IsDBNull(5) ? string.Empty : query.GetString(5);
                    item.Translation = query.IsDBNull(6) ? string.Empty : query.GetString(6);
                    item.Exchange = query.IsDBNull(7) ? string.Empty : query.GetString(7);
                    item.Description = query.IsDBNull(8) ? string.Empty : query.GetString(8);
                    var colorValue = query.IsDBNull(9) ? -1 : query.GetInt32(9);
                    item.Color = (colorValue > 0 && colorValue < 10) ? (GlossaryColorsEnum)colorValue : GlossaryColorsEnum.Transparent;
                    results.Add(item);
                }

                return results;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return null;
        }

        /// <summary>
        /// 获取某个生词本的指定数量的生词，仅支持按照添加时间排序，可以增量加载（按照ID即添加时间增量）
        /// </summary>
        /// <returns></returns>
        public static async Task<List<StarDictWordItem>> GetIncrementalGlossaryWords(int glossaryId, long startId, int limit, string word, GlossaryColorsEnum color, CancellationToken cancellationToken)
        {
            try
            {
                List<StarDictWordItem> results = new List<StarDictWordItem>();

                // 这里是小于号，因为是倒序
                string sql = $"SELECT * FROM glossary WHERE glossaryid=$glossaryid AND id<$id";

                if (!string.IsNullOrWhiteSpace(word))
                {
                    sql += $" AND word LIKE $word";
                }

                if (color != GlossaryColorsEnum.Transparent)
                {
                    sql += $" AND color=$color";
                }

                sql += $" ORDER BY id DESC LIMIT $limit";

                using SqliteCommand selectCommand = new SqliteCommand(sql, _glossaryDb);

                selectCommand.Parameters.AddWithValue("$glossaryid", glossaryId);
                selectCommand.Parameters.AddWithValue("$id", startId);
                selectCommand.Parameters.AddWithValue("$limit", limit);

                if (!string.IsNullOrWhiteSpace(word))
                {
                    selectCommand.Parameters.AddWithValue("$word", word + "%");
                }

                if (color != GlossaryColorsEnum.Transparent)
                {
                    selectCommand.Parameters.AddWithValue("$color", (int)color);
                }

                using SqliteDataReader query = await selectCommand.ExecuteReaderAsync(cancellationToken);
                while (await query.ReadAsync(cancellationToken) == true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    StarDictWordItem item = new StarDictWordItem();
                    item.Id = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    item.Word = query.IsDBNull(3) ? string.Empty : query.GetString(3);
                    item.Phonetic = query.IsDBNull(4) ? string.Empty : query.GetString(4);
                    item.Definition = query.IsDBNull(5) ? string.Empty : query.GetString(5);
                    item.Translation = query.IsDBNull(6) ? string.Empty : query.GetString(6);
                    item.Exchange = query.IsDBNull(7) ? string.Empty : query.GetString(7);
                    item.Description = query.IsDBNull(8) ? string.Empty : query.GetString(8);
                    var colorValue = query.IsDBNull(9) ? -1 : query.GetInt32(9);
                    item.Color = (colorValue > 0 && colorValue < 10) ? (GlossaryColorsEnum)colorValue : GlossaryColorsEnum.Transparent;
                    results.Add(item);
                }

                return results;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return null;
        }

        /// <summary>
        /// 添加一个生词
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static async Task AddGlossaryWord(long wordid, int glossaryId, string word, string phonetic, string definition, string translation, string exchange, string description, GlossaryColorsEnum color)
        {
            try
            {
                using SqliteCommand insertCommand = new SqliteCommand(
                    $"INSERT INTO glossary(wordid,glossaryid,word,phonetic,definition,translation,exchange,description,color) VALUES($wordid,$glossaryid,$word,$phonetic,$definition,$translation,$exchange,$description,$color);",
                    _glossaryDb);
                insertCommand.Parameters.AddWithValue("$wordid", wordid);
                insertCommand.Parameters.AddWithValue("$glossaryid", glossaryId);
                insertCommand.Parameters.AddWithValue("$word", word);
                insertCommand.Parameters.AddWithValue("$phonetic", phonetic);
                insertCommand.Parameters.AddWithValue("$definition", definition);
                insertCommand.Parameters.AddWithValue("$translation", translation);
                insertCommand.Parameters.AddWithValue("$exchange", exchange);
                insertCommand.Parameters.AddWithValue("$description", description);
                insertCommand.Parameters.AddWithValue("$color", (int)color);
                await insertCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 修改生词的属性
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static async Task UpdateGlossaryWord(long id, string description, GlossaryColorsEnum color)
        {
            try
            {
                using SqliteCommand updateCommand = new SqliteCommand(
                    $"UPDATE glossary SET description=$description,color=$color WHERE id=$id;",
                    _glossaryDb);
                updateCommand.Parameters.AddWithValue("$description", description);
                updateCommand.Parameters.AddWithValue("$color", (int)color);
                updateCommand.Parameters.AddWithValue("$id", id);
                await updateCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 删除一个生词
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static async Task DeleteGlossaryWord(long id)
        {
            try
            {
                using SqliteCommand deleteWordsCommand = new SqliteCommand($"DELETE FROM glossary WHERE id=$id;", _glossaryDb);
                deleteWordsCommand.Parameters.AddWithValue("$id", id);
                await deleteWordsCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 查询指定stardictId的单词在指定id的生词本中是否已经存在
        /// </summary>
        /// <param name="wordid"></param>
        /// <param name="glossaryid"></param>
        /// <returns></returns>
        public static async Task<bool> CheckGlossaryContainsWord(long wordid, int glossaryid)
        {
            try
            {
                using SqliteCommand selectWordsCommand = new SqliteCommand($"SELECT 1 FROM glossary WHERE wordid=$wordid AND glossaryid=$glossaryid;", _glossaryDb);
                selectWordsCommand.Parameters.AddWithValue("$wordid", wordid);
                selectWordsCommand.Parameters.AddWithValue("$glossaryid", glossaryid);
                using SqliteDataReader wordsQuery = await selectWordsCommand.ExecuteReaderAsync();
                return await wordsQuery.ReadAsync() == true;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return false;
        }

        #endregion
    }
}
