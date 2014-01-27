using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Error;

namespace MusicBeePlugin.AndroidRemote.Data
{
    /// <summary>
    /// Class CacheHelper.
    /// Is used to handle the library data and cover cache
    /// </summary>
    class CacheHelper
    {
        private const string CREATE_TABLE = "CREATE TABLE \"data\" (" +
                                            "\"hash\" TEXT," +
                                            "\"coverhash\" TEXT," +
                                            "\"filepath\" TEXT);";

        private const string ARTIST_IMAGE_TABLE = "CREATE TABLE \"artist_images\" (" +
                                                  "\"artist\" TEXT," +
                                                  "\"url\" TEXT);";
        private const string DB_NAME = @"\\cache.db";
        private string storagePath;
        private readonly string dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelper"/> class.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        public CacheHelper(string storagePath)
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

                        mCommand.CommandText = ARTIST_IMAGE_TABLE;
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

        public void CreateCache(string[] filenames)
        {
            try
            {
                using (var mConnection = new SQLiteConnection(dbConnection))
                {
                    mConnection.Open();
                    using (var mCommand = new SQLiteCommand(mConnection))
                    using (var mTransaction = mConnection.BeginTransaction())
                    {

                        foreach (var filename in filenames)
                        {
                            mCommand.CommandText = "insert into data(hash,filepath) values (@hash, @filepath);";
                            mCommand.Parameters.AddWithValue("@hash", Utilities.Utilities.Sha1Hash(filename));
                            mCommand.Parameters.AddWithValue("@filepath", filename);
                            mCommand.ExecuteNonQuery();    
                            mCommand.Parameters.Clear();
                        }
                        mTransaction.Commit();
                    }
                    mConnection.Close();
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void UpdateImageCache(List<LibraryData> data)
        {
            try
            {
                using (var mConnection = new SQLiteConnection(dbConnection))
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    mConnection.Open();
                    using (var mCommand = new SQLiteCommand(mConnection))
                    using (var mTransaction = mConnection.BeginTransaction())
                    {
                        foreach (var entry in data)
                        {
                            mCommand.CommandText = "update data set coverhash=@coverhash where hash=@hash";
                            mCommand.Parameters.AddWithValue("@hash", entry.Hash);
                            mCommand.Parameters.AddWithValue("@coverhash", entry.CoverHash);
                            mCommand.ExecuteNonQuery();
                            mCommand.Parameters.Clear();
                        }
                        mTransaction.Commit();
                    }
                    mConnection.Close();
                    sw.Stop();
                    Debug.WriteLine("Update transaction time {0}", sw.Elapsed);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Gets the cached files.
        /// </summary>
        /// <returns>List{LibraryData}.</returns>
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
                        var dataEntry = new LibraryData(mReader["hash"].ToString(), mReader["filepath"].ToString(), mReader["coverhash"].ToString());
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

        /// <summary>
        /// Retrieves a list of all the available hashes for the covers from the cache
        /// </summary>
        /// <returns>List{String}.</returns>
        public List<String> GetCoverHashes()
        {
            List<String> data = new List<string>();
            try
            {
                using (SQLiteConnection mConnection = new SQLiteConnection(dbConnection))
                using (SQLiteCommand mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "select coverhash from data group by coverhash";
                    SQLiteDataReader mReader = mCommand.ExecuteReader();
                    while (mReader.Read())
                    {
                        data.Add(mReader["coverhash"].ToString());
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

        /// <summary>
        /// Updates the cover hash in the cached data for the specified file hash.
        /// </summary>
        /// <param name="filehash">The filehash.</param>
        /// <param name="coverHash">The cover hash.</param>
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

        /// <summary>
        /// Caches the artist URL along with the artist name in the database.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="url">The URL.</param>
        public void CacheArtistUrl(string artist, string url)
        {
            try
            {
                using (SQLiteConnection mConnection = new SQLiteConnection(dbConnection))
                using (SQLiteCommand mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "insert into artist_images (artist, url) values (@artist, @url);";
                    mCommand.Parameters.AddWithValue("@artist", artist);
                    mCommand.Parameters.AddWithValue("@url", url);
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

        /// <summary>
        /// Gets the entry by hash.
        /// </summary>
        /// <param name="hash">The hash sha1 hash of the entry.</param>
        /// <returns>LibraryData.</returns>
        public LibraryData GetEntryByHash(string hash)
        {
            LibraryData data = new LibraryData();
            try
            {
                using (SQLiteConnection mConnection = new SQLiteConnection(dbConnection))
                using (SQLiteCommand mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "select * from data where hash=@hash";
                    mCommand.Parameters.AddWithValue("@hash", hash);
                    SQLiteDataReader mReader = mCommand.ExecuteReader();
                    while (mReader.Read())
                    {
                        data.CoverHash = mReader["coverhash"].ToString();
                        data.Hash = mReader["hash"].ToString();
                        data.Filepath = mReader["filepath"].ToString();
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
    }
}
