using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Error;

namespace MusicBeePlugin.AndroidRemote.Data
{    
    class SqlHelper
    {
        private const string CREATE_TABLE = "CREATE TABLE \"data\" (" +
                                            "\"hash\" TEXT," +
                                            "\"coverhash\" TEXT," +
                                            "\"filepath\" TEXT);";
        private const string DB_NAME = @"\\cache.db";
        private string storagePath;
        private string dbConnection;
        public SqlHelper(string storagePath)
        {
            this.storagePath = storagePath + DB_NAME;
            this.dbConnection = String.Format("Data Source={0}", this.storagePath);
            try
            {
                if (!File.Exists(this.storagePath))
                {
                    SQLiteConnection.CreateFile(this.storagePath);
                    using (SQLiteConnection mConnection = new SQLiteConnection(dbConnection))
                    using (SQLiteCommand mCommand = new SQLiteCommand(mConnection)) 
                    {
                        mConnection.Open();
                        mCommand.CommandText = CREATE_TABLE;
                        mCommand.ExecuteNonQuery();
                        mConnection.Close();
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                ErrorHandler.LogError(e);
#endif
            }
        }

        public void CacheEntry(string hash, string path)
        {
            try
            {
                using (SQLiteConnection mConnection = new SQLiteConnection(dbConnection))
                using (SQLiteCommand mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "insert into data(hash,filepath) values (?, ?);";
                    mCommand.Parameters.AddWithValue("hash", hash);
                    mCommand.Parameters.AddWithValue("filepath", path);
                    mCommand.ExecuteNonQuery();
                    mConnection.Close();
                }
            }
            catch (Exception e)
            {
#if DEBUG
                ErrorHandler.LogError(e);
#endif                

            }
        }

        public List<LibraryData> GetCachedFiles()
        {
            List<LibraryData> data = new List<LibraryData>();
            try
            {
                using (SQLiteConnection mConnection = new SQLiteConnection(dbConnection))
                using (SQLiteCommand mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "select * from data";
                    SQLiteDataReader mReader = mCommand.ExecuteReader();
                    while (mReader.Read())
                    {
                        var dataEntry = new LibraryData(mReader["hash"].ToString(), mReader["filepath"].ToString());
                        data.Add(dataEntry);
                    }
                    mReader.Close();
                    mConnection.Close();
                }
            }
            catch (Exception e)
            {
#if DEBUG
                ErrorHandler.LogError(e);
#endif
            }
            return data;

        }

        public void UpdateCoverHash(string filehash, string coverHash)
        {
            try
            {
                using (SQLiteConnection mConnection = new SQLiteConnection(dbConnection))
                using (SQLiteCommand mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "update data set coverhash=@coverhash where hash=@hash";
                    mCommand.Parameters.AddWithValue("@hash", filehash);
                    mCommand.Parameters.AddWithValue("@coverhash", coverHash);
                    mCommand.ExecuteNonQuery();
                    mConnection.Close();
                }
            }
            catch (Exception e)
            {
#if DEBUG
                ErrorHandler.LogError(e);
#endif
            }
        }

    }
}
