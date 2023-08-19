using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Flint3.Data.Models;
using Microsoft.Data.Sqlite;

namespace Flint3.Data
{
    public static class GlossaryDataAccess
    {
        private static SqliteConnection _glossaryDb = null;

        public static void InitializeDatabase()
        {
            string dbpath = Path.Combine(AppContext.BaseDirectory, "Data/glossary.db");
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
    }
}
