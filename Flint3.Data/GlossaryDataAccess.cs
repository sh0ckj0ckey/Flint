using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flint3.Data.Models;
using Microsoft.Data.Sqlite;

namespace Flint3.Data
{
    public static class GlossaryDataAccess
    {
        private static SqliteConnection? _glossaryDb = null;

        public static async Task LoadDatabase(string dbFilePath)
        {
            _glossaryDb = new SqliteConnection($"Filename={dbFilePath}");
            await _glossaryDb.OpenAsync();

            string categoryTableCommand =
                "CREATE TABLE IF NOT EXISTS glossaryCategory (" +
                    "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                    "title TEXT," +
                    "description TEXT);";
            using SqliteCommand createCategoryTable = new(categoryTableCommand, _glossaryDb);
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
            using SqliteCommand createGlossaryTable = new(glossaryTableCommand, _glossaryDb);
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
        /// 获取所有的生词本
        /// </summary>
        /// <returns>生词本列表</returns>
        public static async Task<List<GlossaryItem>> GetGlossaries()
        {
            try
            {
                List<GlossaryItem> results = [];
                using SqliteCommand selectCommand = new($"SELECT * FROM glossaryCategory", _glossaryDb);
                using SqliteDataReader query = await selectCommand.ExecuteReaderAsync();
                while (await query.ReadAsync() == true)
                {
                    results.Add(new GlossaryItem()
                    {
                        Id = query.IsDBNull(0) ? -1 : query.GetInt32(0),
                        Title = query.IsDBNull(1) ? string.Empty : query.GetString(1),
                        Description = query.IsDBNull(2) ? string.Empty : query.GetString(2)
                    });
                }
                return results;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return [];
        }

        /// <summary>
        /// 添加一个生词本
        /// </summary>
        /// <param name="title">生词本的名称</param>
        /// <param name="desc">生词本的描述</param>
        /// <returns></returns>
        public static async Task AddGlossary(string title, string desc)
        {
            try
            {
                using SqliteCommand insertCommand = new($"INSERT INTO glossaryCategory(title, description) VALUES($title, $desc);", _glossaryDb);
                insertCommand.Parameters.AddWithValue("$title", title);
                insertCommand.Parameters.AddWithValue("$desc", desc);
                await insertCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 修改生词本的属性
        /// </summary>
        /// <param name="glossaryId">生词本ID</param>
        /// <param name="title">生词本的新名称</param>
        /// <param name="desc">生词本的新描述</param>
        /// <returns></returns>
        public static async Task UpdateGlossary(int glossaryId, string title, string desc)
        {
            try
            {
                using SqliteCommand updateCommand = new($"UPDATE glossaryCategory SET title=$title, description=$desc WHERE id=$glossaryid;", _glossaryDb);
                updateCommand.Parameters.AddWithValue("$title", title);
                updateCommand.Parameters.AddWithValue("$desc", desc);
                updateCommand.Parameters.AddWithValue("$glossaryid", glossaryId);
                await updateCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 删除一个生词本，并删除与之关联的所有生词
        /// </summary>
        /// <param name="glossaryId">生词本ID</param>
        /// <returns></returns>
        public static async Task DeleteGlossary(int glossaryId)
        {
            try
            {
                using SqliteCommand deleteCommand = new($"DELETE FROM glossaryCategory WHERE id=$glossaryid;", _glossaryDb);
                deleteCommand.Parameters.AddWithValue("$glossaryid", glossaryId);
                await deleteCommand.ExecuteNonQueryAsync();

                using SqliteCommand deleteWordsCommand = new($"DELETE FROM glossary WHERE glossaryid=$glossaryid;", _glossaryDb);
                deleteWordsCommand.Parameters.AddWithValue("$glossaryid", glossaryId);
                await deleteWordsCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 获取指定生词本的生词总数
        /// </summary>
        /// <param name="glossaryId">生词本ID</param>
        /// <param name="color">筛选生词的颜色，透明表示所有颜色</param>
        /// <returns></returns>
        public static async Task<int> GetGlossaryWordsCount(int glossaryId, GlossaryColorsEnum color)
        {
            try
            {
                using SqliteCommand selectCommand = color != GlossaryColorsEnum.Transparent
                    ? new($"select count(*) from glossary where glossaryid=$glossaryid AND color=$color", _glossaryDb)
                    : new($"select count(*) from glossary where glossaryid=$glossaryid", _glossaryDb);

                selectCommand.Parameters.AddWithValue("$glossaryid", glossaryId);
                if (color != GlossaryColorsEnum.Transparent)
                {
                    selectCommand.Parameters.AddWithValue("$color", (int)color);
                }

                using SqliteDataReader query = await selectCommand.ExecuteReaderAsync();
                while (await query.ReadAsync() == true)
                {
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
        /// 获取生词本的所有生词，按照添加时间或者首字母排序
        /// </summary>
        /// <param name="glossaryId">生词本的ID</param>
        /// <param name="word">需要匹配的生词文本，为空时表示获取所有生词</param>
        /// <param name="color">筛选生词的颜色，透明表示所有颜色</param>
        /// <param name="orderByWord">按照生词排序，否则按照添加时间排序</param>
        /// <param name="cancellationToken"></param>
        /// <returns>完整的生词列表</returns>
        public static async Task<List<StarDictWordItem>> GetAllGlossaryWords(int glossaryId, string word, GlossaryColorsEnum color, bool orderByWord, CancellationToken cancellationToken)
        {
            try
            {
                List<StarDictWordItem> results = [];

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

                using SqliteCommand selectCommand = new(sql, _glossaryDb);

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

                    var colorValue = query.IsDBNull(9) ? -1 : query.GetInt32(9);

                    results.Add(new StarDictWordItem()
                    {
                        Id = query.IsDBNull(0) ? -1 : query.GetInt32(0),
                        Word = query.IsDBNull(3) ? string.Empty : query.GetString(3),
                        Phonetic = query.IsDBNull(4) ? string.Empty : query.GetString(4),
                        Definition = query.IsDBNull(5) ? string.Empty : query.GetString(5),
                        Translation = query.IsDBNull(6) ? string.Empty : query.GetString(6),
                        Exchange = query.IsDBNull(7) ? string.Empty : query.GetString(7),
                        Description = query.IsDBNull(8) ? string.Empty : query.GetString(8),
                        Color = (colorValue > 0 && colorValue < 10) ? (GlossaryColorsEnum)colorValue : GlossaryColorsEnum.Transparent
                    });
                }

                return results;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return [];
        }

        /// <summary>
        /// 增量获取生词本指定数量的生词，仅支持按照添加时间排序
        /// </summary>
        /// <param name="glossaryId">生词本ID</param>
        /// <param name="startId">增量起点生词的最小ID</param>
        /// <param name="limit">增量获取的单词最大数量</param>
        /// <param name="word">需要匹配的生词文本，为空时表示获取所有生词</param>
        /// <param name="color">筛选生词的颜色，透明表示所有颜色</param>
        /// <param name="cancellationToken"></param>
        /// <returns>增量获取的单词列表</returns>
        public static async Task<List<StarDictWordItem>> GetIncrementalGlossaryWords(int glossaryId, long startId, int limit, string word, GlossaryColorsEnum color, CancellationToken cancellationToken)
        {
            try
            {
                List<StarDictWordItem> results = [];

                /* 由于是倒序排列，所以 id 使用小于号 */
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

                using SqliteCommand selectCommand = new(sql, _glossaryDb);

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

                    var colorValue = query.IsDBNull(9) ? -1 : query.GetInt32(9);

                    results.Add(new StarDictWordItem
                    {
                        Id = query.IsDBNull(0) ? -1 : query.GetInt32(0),
                        Word = query.IsDBNull(3) ? string.Empty : query.GetString(3),
                        Phonetic = query.IsDBNull(4) ? string.Empty : query.GetString(4),
                        Definition = query.IsDBNull(5) ? string.Empty : query.GetString(5),
                        Translation = query.IsDBNull(6) ? string.Empty : query.GetString(6),
                        Exchange = query.IsDBNull(7) ? string.Empty : query.GetString(7),
                        Description = query.IsDBNull(8) ? string.Empty : query.GetString(8),
                        Color = (colorValue > 0 && colorValue < 10) ? (GlossaryColorsEnum)colorValue : GlossaryColorsEnum.Transparent
                    });
                }

                return results;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return [];
        }

        /// <summary>
        /// 添加一个生词
        /// </summary>
        /// <param name="wordId">单词在StarDict中的ID</param>
        /// <param name="glossaryId">生词本ID</param>
        /// <param name="word">文本</param>
        /// <param name="phonetic">音标</param>
        /// <param name="definition">定义</param>
        /// <param name="translation">翻译</param>
        /// <param name="exchange">词形变化</param>
        /// <param name="description">描述</param>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        public static async Task AddGlossaryWord(long wordId, int glossaryId, string word, string phonetic, string definition, string translation, string exchange, string description, GlossaryColorsEnum color)
        {
            try
            {
                using SqliteCommand insertCommand = new(
                    $"INSERT INTO glossary(wordid,glossaryid,word,phonetic,definition,translation,exchange,description,color) VALUES($wordid,$glossaryid,$word,$phonetic,$definition,$translation,$exchange,$description,$color);",
                    _glossaryDb);
                insertCommand.Parameters.AddWithValue("$wordid", wordId);
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
        /// <param name="id">生词ID</param>
        /// <param name="description">描述</param>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        public static async Task UpdateGlossaryWord(long id, string description, GlossaryColorsEnum color)
        {
            try
            {
                using SqliteCommand updateCommand = new(
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
        /// <param name="id">生词ID</param>
        /// <returns></returns>
        public static async Task DeleteGlossaryWord(long id)
        {
            try
            {
                using SqliteCommand deleteWordsCommand = new($"DELETE FROM glossary WHERE id=$id;", _glossaryDb);
                deleteWordsCommand.Parameters.AddWithValue("$id", id);
                await deleteWordsCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 查询指定StarDictId的单词在指定ID的生词本中是否已经存在
        /// </summary>
        /// <param name="wordId">单词在StarDict中的ID</param>
        /// <param name="glossaryid">生词本ID</param>
        /// <returns></returns>
        public static async Task<bool> CheckGlossaryContainsWord(long wordId, int glossaryid)
        {
            try
            {
                using SqliteCommand selectWordsCommand = new($"SELECT 1 FROM glossary WHERE wordid=$wordid AND glossaryid=$glossaryid;", _glossaryDb);
                selectWordsCommand.Parameters.AddWithValue("$wordid", wordId);
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
