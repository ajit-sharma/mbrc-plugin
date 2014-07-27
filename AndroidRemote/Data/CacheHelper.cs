using System.Data;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.OrmLite;

namespace MusicBeePlugin.AndroidRemote.Data
{
    using Entities;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;

    /// <summary>
    /// Class CacheHelper.
    /// Is used to handle the library data and cover cache
    /// </summary>
    class CacheHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string DbName = @"\\cache.db";
        private readonly string _dbConnection;
        

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelper"/> class.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        public CacheHelper(string storagePath)
        {
            _dbConnection = storagePath + DbName;
            OrmLiteConfig.DialectProvider = SqliteDialect.Provider;
            try
            {
                using (var db = GetDbConnection())
                {
                    db.CreateTableIfNotExists<LibraryArtist>();
                    db.CreateTableIfNotExists<LibraryAlbum>();
                    db.CreateTableIfNotExists<LibraryGenre>();
                    db.CreateTableIfNotExists<LibraryTrack>();
                }
            
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }
        }

        public IDbConnection GetDbConnection()
        {
            return _dbConnection.OpenDbConnection();
        }

        /// <summary>
        /// Gets the cached entries number.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int GetCachedTracksCount()
        {
            var total = 0;
            try
            {
                using (var db = GetDbConnection())
                {
                    total = db.Select<LibraryTrack>().Count;
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }
            return total;  
        }


        public void CachePlaylists(List<Playlist> playlists)
        {
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                {
                    mConnection.Open();
                    using (var mCommand = new SQLiteCommand(mConnection))
                    using (var mTransaction = mConnection.BeginTransaction())
                    {
                        mCommand.CommandText = "delete from playlists";
                        mCommand.ExecuteNonQuery();
                        mCommand.CommandText = "insert into playlists (name, path, hash) values (@name, @path, @hash);";
                        var nameParam = mCommand.CreateParameter();
                        var pathParam = mCommand.CreateParameter();
                        var hashParam = mCommand.CreateParameter();
                        nameParam.ParameterName = "@name";
                        pathParam.ParameterName = "@path";
                        hashParam.ParameterName = "@hash";
                        mCommand.Parameters.Add(nameParam);
                        mCommand.Parameters.Add(pathParam);
                        mCommand.Parameters.Add(hashParam);

                        foreach (var playlist in playlists)
                        {
                            nameParam.Value = playlist.name;
                            pathParam.Value = playlist.path;
                            hashParam.Value = playlist.hash;
                            mCommand.ExecuteNonQuery();
                        }
                        mTransaction.Commit();
                    }
                    mConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
            }
        }

        public Playlist GetPlaylistByHash(string hash)
        {
            var playlist = new Playlist();
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                {
                    mConnection.Open();
                    using (var mCommand = new SQLiteCommand(mConnection))
                    {
                        mCommand.CommandText = "select * from playlists where hash=@hash";
                        mCommand.Parameters.AddWithValue("@hash", hash);
                        SQLiteDataReader mReader = mCommand.ExecuteReader();
                        while (mReader.Read())
                        {
                            playlist.hash = mReader["hash"].ToString();
                            playlist.name = mReader["name"].ToString();
                            playlist.path = mReader["path"].ToString();
                        }
                        mReader.Close();
                    }
                    mConnection.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }
            return playlist;
        }


        /// <summary>
        /// Adds the new files to cache.
        /// </summary>
        /// <param name="files">The paths to the files added in the cache.</param>
        public void AddNewFilesToCache(string[] files)
        {
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                {
                    mConnection.Open();
                    using (var mCommand = new SQLiteCommand(mConnection))
                    using (var mTransaction = mConnection.BeginTransaction())
                    {
                        mCommand.CommandText = "insert into data (hash, filepath, updated) values (@hash, @filepath, @updated);";
                        var hashParam = mCommand.CreateParameter();
                        var fileParam = mCommand.CreateParameter();
                        var updatedParam = mCommand.CreateParameter();
                        hashParam.ParameterName = "@hash";
                        fileParam.ParameterName = "@filepath";
                        updatedParam.ParameterName = "@updated";
                        mCommand.Parameters.Add(hashParam);
                        mCommand.Parameters.Add(fileParam);
                        mCommand.Parameters.Add(updatedParam);

                        foreach (var filename in files)
                        {
                            hashParam.Value = Utilities.Utilities.Sha1Hash(filename);
                            fileParam.Value = filename;
                            updatedParam.Value = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                            mCommand.ExecuteNonQuery();    
                        }
                        mTransaction.Commit();
                    }
                    mConnection.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }
        }

        /// <summary>
        /// Deletes the files from cache.
        /// </summary>
        /// <param name="files">The files.</param>
        public void DeleteFilesFromCache(string[] files)
        {
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                {
                    mConnection.Open();
                    using (var mCommand = new SQLiteCommand(mConnection))
                    using (var mTransaction = mConnection.BeginTransaction())
                    {
                        mCommand.CommandText = "delete from data where filepath=@filepath";
                        var fileParam = mCommand.CreateParameter();
                        fileParam.ParameterName = "@filepath";
                        mCommand.Parameters.Add(fileParam);
                        
                        foreach (var filename in files)
                        {
                            fileParam.Value = filename;   
                            mCommand.ExecuteNonQuery();
                        }
                        mTransaction.Commit();
                    }
                    mConnection.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }
        }

        /// <summary>
        /// Marks the files as updated in the cache.
        /// </summary>
        /// <param name="files">The files.</param>
        public void MarkFilesUpdated(string[] files)
        {
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                {
                    mConnection.Open();
                    using (var mCommand = new SQLiteCommand(mConnection))
                    using (var mTransaction = mConnection.BeginTransaction())
                    {
                        mCommand.CommandText = "update data set updated=@updated where filepath=@filepath";
                        var fileParam = mCommand.CreateParameter();
                        var updatedParam = mCommand.CreateParameter();
                        fileParam.ParameterName = "@filepath";
                        updatedParam.ParameterName = "@updated";
                        mCommand.Parameters.Add(fileParam);
                        mCommand.Parameters.Add(updatedParam);
                        foreach (var filename in files)
                        {
                            fileParam.Value = filename;
                            updatedParam.Value = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                            mCommand.ExecuteNonQuery();
                        }
                        mTransaction.Commit();
                    }
                    mConnection.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }
        }


        /// <summary>
        /// Gets the cached files.
        /// </summary>
        /// <returns>List{LibraryData}.</returns>
        public List<LibraryData> GetCachedFiles()
        {
            var data = new List<LibraryData>();
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                using (var mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "select * from data";
                    var mReader = mCommand.ExecuteReader();
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
                Logger.Debug(e);
            }
            return data;
        }

        /// <summary>
        /// Gets the total number of covers available.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int GetCachedCoversCount()
        {
            var total = 0;
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                using (var mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "select count(*) as count from covers";
                    var mReader = mCommand.ExecuteReader();
                    while (mReader.Read())
                    {
                        total = int.Parse(mReader["count"].ToString());
                    }
                    mReader.Close();
                    mConnection.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }
            return total;  
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
                using (var mConnection = new SQLiteConnection(_dbConnection))
                using (var mCommand = new SQLiteCommand(mConnection))
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
                Logger.Debug(e);
            }
        }

        /// <summary>
        /// Returns the date of the latest metadata entry update.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime GetLastMetaDataUpdate()
        {
            var date = DateTime.UtcNow;
            try
            {
                using(var mConnection = new SQLiteConnection(_dbConnection))
                using (var mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "select updated from data order by updated desc limit 1";
                    var mReader = mCommand.ExecuteReader();
                    mReader.Read();
                    date = DateTime.Parse(mReader["updated"].ToString());
                    mConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
            }
            return date;
        }

        /// <summary>
        /// Gets the entry by hash.
        /// </summary>
        /// <param name="hash">The hash sha1 hash of the entry.</param>
        /// <returns>LibraryData.</returns>
        public LibraryData GetEntryByHash(string hash)
        {
            var data = new LibraryData();
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                using (var mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "select * from data where hash=@hash";
                    mCommand.Parameters.AddWithValue("@hash", hash);
                    var mReader = mCommand.ExecuteReader();
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
                Logger.Debug(e);
            }
            return data;
        }
    }
}
