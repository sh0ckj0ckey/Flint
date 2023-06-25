using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flint.Data.Models;
using Microsoft.Data.Sqlite;
using Windows.Storage;
using static System.Net.Mime.MediaTypeNames;

namespace Flint.Data
{
    public static class StarDictDataAccess
    {
        private static SqliteConnection _starDictDb = null;

        public static void InitializeDatabase()
        {
            string dbpath = "Data\\stardict.db";
            _starDictDb = new SqliteConnection($"Filename={dbpath}");
            _starDictDb.Open();

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
            }
            catch { }
        }

        /// <summary>
        /// 查询单词
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static List<StarDictWordItem> QueryWord(string word)
        {
            try
            {
                List<StarDictWordItem> results = new List<StarDictWordItem>();
                SqliteCommand selectCommand = new SqliteCommand($"select * from stardict where word = $word", _starDictDb);
                selectCommand.Parameters.AddWithValue("$word", word);
                SqliteDataReader query = selectCommand.ExecuteReader();
                while (query.Read())
                {
                    StarDictWordItem item = new StarDictWordItem();
                    item.Id = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    item.Word = query.IsDBNull(1) ? string.Empty : query.GetString(1);
                    item.StripWord = query.IsDBNull(2) ? string.Empty : query.GetString(2);
                    item.Phonetic = query.IsDBNull(3) ? string.Empty : query.GetString(3);
                    item.Definition = query.IsDBNull(4) ? string.Empty : query.GetString(4);
                    item.Translation = query.IsDBNull(5) ? string.Empty : query.GetString(5);
                    item.Exchange = query.IsDBNull(12) ? string.Empty : query.GetString(12);
                    results.Add(item);
                }
                return results;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 匹配单词
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static List<StarDictWordItem> MatchWord(string word)
        {
            try
            {
                List<StarDictWordItem> results = new List<StarDictWordItem>();
                SqliteCommand selectCommand = new SqliteCommand($"select * from stardict where sw >= $word order by sw, word collate nocase limit $limit", _starDictDb);
                selectCommand.Parameters.AddWithValue("$word", word);
                selectCommand.Parameters.AddWithValue("$limit", 10);
                SqliteDataReader query = selectCommand.ExecuteReader();
                while (query.Read())
                {
                    StarDictWordItem item = new StarDictWordItem();
                    item.Id = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    item.Word = query.IsDBNull(1) ? string.Empty : query.GetString(1);
                    item.StripWord = query.IsDBNull(2) ? string.Empty : query.GetString(2);
                    item.Phonetic = query.IsDBNull(3) ? string.Empty : query.GetString(3);
                    item.Definition = query.IsDBNull(4) ? string.Empty : query.GetString(4);
                    item.Translation = query.IsDBNull(5) ? string.Empty : query.GetString(5);
                    item.Exchange = query.IsDBNull(12) ? string.Empty : query.GetString(12);
                    results.Add(item);
                }
                return results;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 获取当前数据库单词总数
        /// </summary>
        /// <returns></returns>
        public static long GetCount()
        {
            try
            {
                SqliteCommand selectCommand = new SqliteCommand($"select count(*) from stardict;", _starDictDb);
                SqliteDataReader query = selectCommand.ExecuteReader();
                while (query.Read())
                {
                    StarDictWordItem item = new StarDictWordItem();
                    var count = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    return count;
                }
            }
            catch { }
            return -1;
        }
    }
}
