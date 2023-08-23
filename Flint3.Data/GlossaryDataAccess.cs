using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Flint3.Data.Models;
using Microsoft.Data.Sqlite;
using Windows.Storage;

namespace Flint3.Data
{
    public static class GlossaryDataAccess
    {
        private static SqliteConnection _glossaryDb = null;

        public static void LoadDatabase(StorageFolder folder/*string filePath*/)
        {
            var file = folder.CreateFileAsync("flint_glossary.db", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();

            string dbpath = file.Path;
            _glossaryDb = new SqliteConnection($"Filename={dbpath}");
            _glossaryDb.Open();

            string categoryTableCommand =
                "CREATE TABLE IF NOT EXISTS glossaryCategory (" +
                    "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                    "title TEXT," +
                    "description TEXT);";
            SqliteCommand createCategoryTable = new SqliteCommand(categoryTableCommand, _glossaryDb);
            createCategoryTable.ExecuteReader();

            string glossaryTableCommand =
                "CREATE TABLE IF NOT EXISTS glossary (" +
                    "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                    "wordid INTEGER(64) NOT NULL," +
                    "glossaryid INTEGER(64) NOT NULL," +
                    "word VARCHAR(64) COLLATE NOCASE NOT NULL UNIQUE," +
                    "phonetic VARCHAR(64)," +
                    "definition TEXT," +
                    "translation TEXT," +
                    "exchange TEXT," +
                    "description TEXT," +
                    "color INTEGER);";
            SqliteCommand createGlossaryTable = new SqliteCommand(glossaryTableCommand, _glossaryDb);
            createGlossaryTable.ExecuteReader();
        }

        public static void CloseDatabase()
        {
            _glossaryDb?.Close();
            _glossaryDb?.Dispose();
            _glossaryDb = null;
        }

        #region 生词本

        /// <summary>
        /// 获取所有的生词本列表
        /// </summary>
        /// <returns></returns>
        public static List<GlossaryItem> GetAllGlossaries()
        {
            try
            {
                List<GlossaryItem> results = new List<GlossaryItem>();
                SqliteCommand selectCommand = new SqliteCommand($"SELECT * FROM glossaryCategory", _glossaryDb);
                SqliteDataReader query = selectCommand?.ExecuteReader();
                while (query?.Read() == true)
                {
                    GlossaryItem item = new GlossaryItem();
                    item.Id = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    item.Title = query.IsDBNull(1) ? string.Empty : query.GetString(1);
                    item.Description = query.IsDBNull(2) ? string.Empty : query.GetString(2);
                    results.Add(item);
                }
                return results;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 添加一个生词本
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static void AddOneGlossary(string title, string desc)
        {
            try
            {
                SqliteCommand insertCommand = new SqliteCommand($"INSERT INTO glossaryCategory(title, description) VALUES($title, $desc);", _glossaryDb);
                insertCommand.Parameters.AddWithValue("$title", title);
                insertCommand.Parameters.AddWithValue("$desc", desc);
                SqliteDataReader query = insertCommand?.ExecuteReader();
            }
            catch { }
        }

        /// <summary>
        /// 修改生词本的属性
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static void UpdateOneGlossary(int id, string title, string desc)
        {
            try
            {
                SqliteCommand updateCommand = new SqliteCommand($"UPDATE glossaryCategory SET title=$title, description=$desc WHERE id=$glossaryid;", _glossaryDb);
                updateCommand.Parameters.AddWithValue("$title", title);
                updateCommand.Parameters.AddWithValue("$desc", desc);
                updateCommand.Parameters.AddWithValue("$glossaryid", id);
                SqliteDataReader query = updateCommand?.ExecuteReader();
            }
            catch { }
        }

        /// <summary>
        /// 删除一个生词本，并删除glossary表中所有关联的生词
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static void DeleteOneGlossary(int id)
        {
            try
            {
                SqliteCommand deleteCommand = new SqliteCommand($"DELETE FROM glossaryCategory WHERE id=$glossaryid;", _glossaryDb);
                deleteCommand.Parameters.AddWithValue("$glossaryid", id);
                SqliteDataReader query = deleteCommand?.ExecuteReader();

                SqliteCommand deleteWordsCommand = new SqliteCommand($"DELETE FROM glossary WHERE glossaryid=$glossaryid;", _glossaryDb);
                deleteWordsCommand.Parameters.AddWithValue("$glossaryid", id);
                SqliteDataReader wordsQuery = deleteWordsCommand?.ExecuteReader();
            }
            catch { }
        }

        #endregion

        #region 生词

        /// <summary>
        /// 获取某个生词本的指定数量的生词
        /// </summary>
        /// <returns></returns>
        public static List<GlossaryWordItem> GetGlossaryWords(int glossaryId, long startId, int limit, string word)
        {
            try
            {
                List<GlossaryWordItem> results = new List<GlossaryWordItem>();
                SqliteCommand selectCommand = null;

                if (string.IsNullOrWhiteSpace(word))
                {
                    selectCommand = new SqliteCommand($"SELECT * FROM glossary WHERE glossaryid=$glossaryid AND id>$id order by word collate nocase limit $limit", _glossaryDb);
                }
                else
                {
                    selectCommand = new SqliteCommand($"SELECT * FROM glossary WHERE glossaryid=$glossaryid AND id>$id and word like $word order by word collate nocase limit $limit", _glossaryDb);
                    selectCommand.Parameters.AddWithValue("$word", word + "%");
                }

                selectCommand.Parameters.AddWithValue("$glossaryid", glossaryId);
                selectCommand.Parameters.AddWithValue("$id", startId);
                selectCommand.Parameters.AddWithValue("$limit", limit);
                SqliteDataReader query = selectCommand?.ExecuteReader();

                while (query?.Read() == true)
                {
                    GlossaryWordItem item = new GlossaryWordItem();
                    item.Id = query.IsDBNull(0) ? -1 : query.GetInt32(0);
                    item.Word = query.IsDBNull(1) ? string.Empty : query.GetString(2);
                    item.Phonetic = query.IsDBNull(3) ? string.Empty : query.GetString(3);
                    item.Definition = query.IsDBNull(4) ? string.Empty : query.GetString(4);
                    item.Translation = query.IsDBNull(5) ? string.Empty : query.GetString(5);
                    item.Exchange = query.IsDBNull(12) ? string.Empty : query.GetString(6);
                    item.Description = query.IsDBNull(12) ? string.Empty : query.GetString(7);
                    item.Color = query.IsDBNull(12) ? -1 : query.GetInt32(8);
                    results.Add(item);
                }
                return results;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 添加一个生词
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static void AddGlossaryWord(long wordid, int glossaryId, string word, string phonetic, string definition, string translation, string exchange, string description, int color)
        {
            try
            {
                SqliteCommand insertCommand = new SqliteCommand(
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
                insertCommand.Parameters.AddWithValue("$color", color);
                SqliteDataReader query = insertCommand?.ExecuteReader();
            }
            catch { }
        }

        /// <summary>
        /// 修改生词的属性
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static void UpdateGlossaryWord(int id, string word, string phonetic, string definition, string translation, string exchange, string description, int color)
        {
            try
            {
                SqliteCommand updateCommand = new SqliteCommand(
                    $"UPDATE glossary SET description=$description,color=$color WHERE id=$id;",
                    _glossaryDb);
                updateCommand.Parameters.AddWithValue("$description", description);
                updateCommand.Parameters.AddWithValue("$color", color);
                updateCommand.Parameters.AddWithValue("$id", id);
                SqliteDataReader query = updateCommand?.ExecuteReader();
            }
            catch { }
        }

        /// <summary>
        /// 删除一个生词
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        public static void DeleteGlossaryWord(int id)
        {
            try
            {
                SqliteCommand deleteWordsCommand = new SqliteCommand($"DELETE FROM glossary WHERE id=$id;", _glossaryDb);
                deleteWordsCommand.Parameters.AddWithValue("$id", id);
                SqliteDataReader wordsQuery = deleteWordsCommand?.ExecuteReader();
            }
            catch { }
        }

        /// <summary>
        /// 查询指定stardictId的单词在指定id的生词本中是否已经存在
        /// </summary>
        /// <param name="wordid"></param>
        /// <param name="glossaryid"></param>
        /// <returns></returns>
        public static bool IfExistGlossaryWord(long wordid, int glossaryid)
        {
            try
            {
                SqliteCommand selectWordsCommand = new SqliteCommand($"SELECT 1 FROM glossary WHERE wordid=$wordid AND glossaryid=$glossaryid;", _glossaryDb);
                selectWordsCommand.Parameters.AddWithValue("$wordid", wordid);
                SqliteDataReader wordsQuery = selectWordsCommand?.ExecuteReader();

                return wordsQuery?.Read() == true;
            }
            catch { }
            return false;
        }

        #endregion
    }
}
