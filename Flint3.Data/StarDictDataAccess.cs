using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Flint3.Data.Models;
using Microsoft.Data.Sqlite;

namespace Flint3.Data
{
    public static class StarDictDataAccess
    {
        private static SqliteConnection? _starDictDb = null;

        public static async Task InitializeDatabase()
        {
            _starDictDb = new SqliteConnection($"Filename={Path.Combine(AppContext.BaseDirectory, "Data/stardict.db")}");
            await _starDictDb.OpenAsync();

            //string tableCommand =
            //    "CREATE TABLE IF NOT EXISTS stardict (" +
            //        "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
            //        "word VARCHAR(64) COLLATE NOCASE NOT NULL UNIQUE," +
            //        "sw VARCHAR(64) COLLATE NOCASE NOT NULL," +
            //        "phonetic VARCHAR(64)," +
            //        "definition TEXT," +
            //        "translation TEXT," +
            //        "pos VARCHAR(16)," +
            //        "collins INTEGER DEFAULT(0)," +
            //        "oxford INTEGER DEFAULT(0)," +
            //        "tag VARCHAR(64)," +
            //        "bnc INTEGER DEFAULT(NULL)," +
            //        "frq INTEGER DEFAULT(NULL)," +
            //        "exchange TEXT," +
            //        "detail TEXT," +
            //        "audio TEXT);" +
            //    "CREATE UNIQUE INDEX IF NOT EXISTS stardict_1 ON stardict(id);" +
            //    "CREATE UNIQUE INDEX IF NOT EXISTS stardict_2 ON stardict(word);" +
            //    "CREATE INDEX IF NOT EXISTS stardict_3 ON stardict(sw, word collate nocase);" +
            //    "CREATE INDEX IF NOT EXISTS sd_1 ON stardict(word collate nocase); ";
            //SqliteCommand createTable = new SqliteCommand(tableCommand, db);
            //createTable.ExecuteReader();
        }

        public static void CloseDatabase()
        {
            try
            {
                _starDictDb?.Close();
                _starDictDb?.Dispose();
                _starDictDb = null;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 查找单词
        /// </summary>
        /// <param name="word">精确查找的单词文本</param>
        /// <returns>所有的完全匹配的单词</returns>
        public static async Task<List<StarDictWordItem>> QueryWord(string word)
        {
            try
            {
                List<StarDictWordItem> results = [];
                using SqliteCommand selectCommand = new($"select * from stardict where word = $word", _starDictDb);
                selectCommand.Parameters.AddWithValue("$word", word);
                using SqliteDataReader query = await selectCommand.ExecuteReaderAsync();
                while (await query.ReadAsync() == true)
                {
                    results.Add(new StarDictWordItem
                    {
                        Id = query.IsDBNull(0) ? -1 : query.GetInt32(0),
                        Word = query.IsDBNull(1) ? string.Empty : query.GetString(1),
                        StripWord = query.IsDBNull(2) ? string.Empty : query.GetString(2),
                        Phonetic = query.IsDBNull(3) ? string.Empty : query.GetString(3),
                        Definition = query.IsDBNull(4) ? string.Empty : query.GetString(4),
                        Translation = query.IsDBNull(5) ? string.Empty : query.GetString(5),
                        Exchange = query.IsDBNull(12) ? string.Empty : query.GetString(12)
                    });
                }
                return results;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return [];
        }

        /// <summary>
        /// 匹配单词
        /// </summary>
        /// <param name="word">前缀匹配的单词文本</param>
        /// <param name="limit">查询结果条数限制</param>
        /// <returns>特定数量的前缀匹配的单词</returns>
        public static async Task<List<StarDictWordItem>> MatchWord(string word, int limit)
        {
            try
            {
                List<StarDictWordItem> results = [];
                using SqliteCommand selectCommand = new($"select * from stardict where sw >= $word order by sw, word collate nocase limit $limit", _starDictDb);
                selectCommand.Parameters.AddWithValue("$word", word);
                selectCommand.Parameters.AddWithValue("$limit", limit);
                using SqliteDataReader query = await selectCommand.ExecuteReaderAsync();
                while (await query.ReadAsync() == true)
                {
                    results.Add(new StarDictWordItem
                    {
                        Id = query.IsDBNull(0) ? -1 : query.GetInt64(0),
                        Word = query.IsDBNull(1) ? string.Empty : query.GetString(1),
                        StripWord = query.IsDBNull(2) ? string.Empty : query.GetString(2),
                        Phonetic = query.IsDBNull(3) ? string.Empty : query.GetString(3),
                        Definition = query.IsDBNull(4) ? string.Empty : query.GetString(4),
                        Translation = query.IsDBNull(5) ? string.Empty : query.GetString(5),
                        Exchange = query.IsDBNull(12) ? string.Empty : query.GetString(12)
                    });
                }
                return results;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return [];
        }

        #region 扩展生词本

        /// <summary>
        /// 获取内置生词本的所有单词
        /// </summary>
        /// <param name="tag">内置生词本标签</param>
        /// <param name="word">需要匹配的单词文本，为空时表示获取所有单词</param>
        /// <param name="cancellationToken"></param>
        /// <returns>完整的单词列表</returns>
        public static async Task<List<StarDictWordItem>> GetAllExtraGlossaryWords(string tag, string word, CancellationToken cancellationToken)
        {
            SqliteCommand? selectCommand = null;

            try
            {
                List<StarDictWordItem> results = [];
                if (tag != "oxford")
                {
                    if (string.IsNullOrWhiteSpace(word))
                    {
                        selectCommand = new SqliteCommand($"select * from stardict where tag like $tag order by word collate nocase", _starDictDb);
                    }
                    else
                    {
                        selectCommand = new SqliteCommand($"select * from stardict where tag like $tag and word like $word order by word collate nocase", _starDictDb);
                        selectCommand.Parameters.AddWithValue("$word", word + "%");
                    }
                    selectCommand.Parameters.AddWithValue("$tag", "%" + tag + "%");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(word))
                    {
                        selectCommand = new SqliteCommand($"select * from stardict where oxford = 1 order by word collate nocase", _starDictDb);
                    }
                    else
                    {
                        selectCommand = new SqliteCommand($"select * from stardict where oxford = 1 and word like $word order by word collate nocase", _starDictDb);
                        selectCommand.Parameters.AddWithValue("$word", word + "%");
                    }
                }

                using SqliteDataReader query = await selectCommand.ExecuteReaderAsync(cancellationToken);
                while (await query.ReadAsync(cancellationToken) == true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    results.Add(new StarDictWordItem
                    {
                        Id = query.IsDBNull(0) ? -1 : query.GetInt64(0),
                        Word = query.IsDBNull(1) ? string.Empty : query.GetString(1),
                        StripWord = query.IsDBNull(2) ? string.Empty : query.GetString(2),
                        Phonetic = query.IsDBNull(3) ? string.Empty : query.GetString(3),
                        Definition = query.IsDBNull(4) ? string.Empty : query.GetString(4),
                        Translation = query.IsDBNull(5) ? string.Empty : query.GetString(5),
                        Exchange = query.IsDBNull(12) ? string.Empty : query.GetString(12)
                    });
                }

                return results;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            finally
            {
                selectCommand?.Dispose();
            }

            return [];
        }

        /// <summary>
        /// 增量获取内置生词本指定数量的单词
        /// </summary>
        /// <param name="tag">内置生词本标签</param>
        /// <param name="startId">增量起点单词的最小id</param>
        /// <param name="limit">增量获取的单词最大数量</param>
        /// <param name="word">需要匹配的单词文本，为空时表示获取所有单词</param>
        /// <param name="cancellationToken"></param>
        /// <returns>增量获取的单词列表</returns>
        public static async Task<List<StarDictWordItem>> GetIncrementalExtraGlossaryWords(string tag, long startId, int limit, string word, CancellationToken cancellationToken)
        {
            SqliteCommand? selectCommand = null;

            try
            {
                List<StarDictWordItem> results = [];
                if (tag != "oxford")
                {
                    if (string.IsNullOrWhiteSpace(word))
                    {
                        selectCommand = new SqliteCommand($"select * from stardict where tag like $tag and id > $id order by word collate nocase limit $limit", _starDictDb);
                    }
                    else
                    {
                        selectCommand = new SqliteCommand($"select * from stardict where tag like $tag and id > $id and word like $word order by word collate nocase limit $limit", _starDictDb);
                        selectCommand.Parameters.AddWithValue("$word", word + "%");
                    }
                    selectCommand.Parameters.AddWithValue("$id", startId);
                    selectCommand.Parameters.AddWithValue("$tag", "%" + tag + "%");
                    selectCommand.Parameters.AddWithValue("$limit", limit);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(word))
                    {
                        selectCommand = new SqliteCommand($"select * from stardict where oxford = 1 and id > $id order by word collate nocase limit $limit", _starDictDb);
                    }
                    else
                    {
                        selectCommand = new SqliteCommand($"select * from stardict where oxford = 1 and id > $id and word like $word order by word collate nocase limit $limit", _starDictDb);
                        selectCommand.Parameters.AddWithValue("$word", word + "%");
                    }
                    selectCommand.Parameters.AddWithValue("$id", startId);
                    selectCommand.Parameters.AddWithValue("$limit", limit);
                }

                using SqliteDataReader query = await selectCommand.ExecuteReaderAsync(cancellationToken);
                while (await query.ReadAsync(cancellationToken) == true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    results.Add(new StarDictWordItem
                    {
                        Id = query.IsDBNull(0) ? -1 : query.GetInt64(0),
                        Word = query.IsDBNull(1) ? string.Empty : query.GetString(1),
                        StripWord = query.IsDBNull(2) ? string.Empty : query.GetString(2),
                        Phonetic = query.IsDBNull(3) ? string.Empty : query.GetString(3),
                        Definition = query.IsDBNull(4) ? string.Empty : query.GetString(4),
                        Translation = query.IsDBNull(5) ? string.Empty : query.GetString(5),
                        Exchange = query.IsDBNull(12) ? string.Empty : query.GetString(12)
                    });
                }

                return results;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            finally
            {
                selectCommand?.Dispose();
            }

            return [];
        }

        /// <summary>
        /// 获取指定生词本的单词总数
        /// </summary>
        /// <param name="tag">内置生词本标签</param>
        /// <returns>单词数量</returns>
        public static async Task<int> GetExtraGlossaryWordsCount(string tag)
        {
            try
            {
                using SqliteCommand selectCommand = tag != "oxford"
                    ? new SqliteCommand($"select count(*) from stardict where tag LIKE $tag;", _starDictDb)
                    : new SqliteCommand($"select count(*) from stardict where oxford = 1;", _starDictDb);

                if (tag != "oxford")
                {
                    selectCommand.Parameters.AddWithValue("$tag", "%" + tag + "%");
                }

                using SqliteDataReader query = await selectCommand.ExecuteReaderAsync();
                while (await query.ReadAsync() == true)
                {
                    StarDictWordItem item = new();
                    var count = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    return count;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return -1;
        }

        #endregion

    }
}
