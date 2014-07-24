using System.Security.Policy;

namespace MusicBeePlugin.AndroidRemote.Data
{
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using Entities;

    /// <summary>
    /// Class CacheHelper.
    /// Is used to handle the library data and cover cache
    /// </summary>
    class CacheHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string CreateTable = "CREATE TABLE \"data\" (" +
                                            "\"_id\" integer primary key," +
                                            "\"hash\" TEXT," +
                                            "\"updated\" TEXT," +
                                            "\"filepath\" TEXT);";

        private const string ArtistImageTable = "CREATE TABLE \"artist_images\" (" +
                                                  "\"_id\" integer primary key," +
                                                  "\"artist\" TEXT," +
                                                  "\"updated\" TEXT," +
                                                  "\"url\" TEXT);";

        private const string PlaylistTable = "create table \"playlists\" (" +
                                              "\"_id\" integer primary key," +
                                              "\"name\" text," +
                                              "\"path\" text," +
                                              "\"updated\" text," +
                                              "\"hash\" text)";

        private const string CoverCacheTable = "create table \"covers\" (" +
                                                 "\"_id\" integer primary key," +
                                                 "\"coverhash\" text," +
                                                 "\"updated\" text," +
                                                 "\"album_id\" text)";

        private const string NowPlayingTracks = "CREATE TABLE \"now_playing\" (" +
                                                "\"_id\" integer primary key," +
                                                "\"position\" integer," +
                                                "\"title\" text," +
                                                "\"artist\" text," +
                                                "\"hash\" text)";



        private const string DbName = @"\\cache.db";
        
        private readonly string _dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelper"/> class.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        public CacheHelper(string storagePath)
        {
            storagePath = storagePath + DbName;
            _dbConnection = String.Format("Data Source={0}", storagePath);
            try
            {
                if (File.Exists(storagePath)) return;
                SQLiteConnection.CreateFile(storagePath);
                using (var mConnection = new SQLiteConnection(_dbConnection))
                using (var mCommand = new SQLiteCommand(mConnection)) 
                {
                    mConnection.Open();
                    mCommand.CommandText = CreateTable;
                    mCommand.ExecuteNonQuery();

                    mCommand.CommandText = ArtistImageTable;
                    mCommand.ExecuteNonQuery();

                    mCommand.CommandText = PlaylistTable;
                    mCommand.ExecuteNonQuery();
                        
                    mCommand.CommandText = CoverCacheTable;
                    mCommand.ExecuteNonQuery();

                    mCommand.CommandText = NowPlayingTracks;
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
        /// Gets the cached entries number.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int GetCachedTracksCount()
        {
            var total = 0;
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                using (var mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "select count(*) as count from data";
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
        /// Caches the now playing tracks.
        /// </summary>
        /// <param name="tracks">The tracks.</param>
        public void CacheNowPlayingTracks(List<NowPlayingListTrack> tracks)
        {
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                {
                    mConnection.Open();
                    using (var mCommand = new SQLiteCommand(mConnection))
                    using (var mTransaction = mConnection.BeginTransaction())
                    {
                        mCommand.CommandText = "delete from now_playing";
                        mCommand.ExecuteNonQuery();
                        mCommand.CommandText =
                            "insert into now_playing (position, artist, title, hash) values (@position, @artist, @title, @hash)";;
                        var positionParam = mCommand.CreateParameter();
                        var artistParam = mCommand.CreateParameter();
                        var titleParam = mCommand.CreateParameter();
                        var hashParam = mCommand.CreateParameter();
                        positionParam.ParameterName = "@position";
                        artistParam.ParameterName = "@artist";
                        titleParam.ParameterName = "@title";
                        hashParam.ParameterName = "@hash";
                        mCommand.Parameters.Add(positionParam);
                        mCommand.Parameters.Add(artistParam);
                        mCommand.Parameters.Add(titleParam);
                        mCommand.Parameters.Add(hashParam);

                        foreach (var nowPlayingListTrack in tracks)
                        {
                            positionParam.Value = nowPlayingListTrack.position;
                            artistParam.Value = nowPlayingListTrack.artist;
                            titleParam.Value = nowPlayingListTrack.title;
                            hashParam.Value = nowPlayingListTrack.hash;
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

        public long GetCurrentQueueTotalTracks()
        {
            var total = 0;
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                using (var mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "select count(*) as count from now_playing";
                    var mReader = mCommand.ExecuteReader();
                    while (mReader.Read())
                    {
                        total = int.Parse(mReader["count"].ToString());
                    }
                    mReader.Close();
                    mConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
            }
            return total;
        }

        public List<NowPlayingListTrack> GetNowPlayingListTracks(int offset, int limit)
        {
            var data = new List<NowPlayingListTrack>();
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                using (var mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "select * from now_playing limit @limit offset @offset";
                    var limitParam = mCommand.CreateParameter();
                    var offsetParam = mCommand.CreateParameter();
                    limitParam.ParameterName = "@limit";
                    offsetParam.ParameterName = "@offset";
                    limitParam.Value = limit;
                    offsetParam.Value = offset;
                    mCommand.Parameters.Add(limitParam);
                    mCommand.Parameters.Add(offsetParam);
                    var mReader = mCommand.ExecuteReader();
                    while (mReader.Read())
                    {
                        var entry = new NowPlayingListTrack(mReader["artist"].ToString(),
                            mReader["title"].ToString(),
                            int.Parse(mReader["position"].ToString()),
                            mReader["hash"].ToString());

                        data.Add(entry);
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


        public void BuildImageCache(List<AlbumEntry> data)
        {
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                {
                    mConnection.Open();
                    using (var mCommand = new SQLiteCommand(mConnection))
                    using (var mTransaction = mConnection.BeginTransaction())
                    {
                        mCommand.CommandText = "delete from covers";
                        mCommand.ExecuteNonQuery();

                        var cHashParam = mCommand.CreateParameter();
                        var albumIdParam = mCommand.CreateParameter();
                        var updated = mCommand.CreateParameter();
                        mCommand.CommandText = "insert into covers(coverhash, album_id, updated) values (@coverhash, @album_id, @updated);";
                        cHashParam.ParameterName = "@coverhash";
                        albumIdParam.ParameterName = "@album_id";
                        updated.ParameterName = "@updated";
                        mCommand.Parameters.Add(cHashParam);
                        mCommand.Parameters.Add(albumIdParam);
                        mCommand.Parameters.Add(updated);

                        foreach (var entry in data)
                        {
                            cHashParam.Value = entry.CoverHash;
                            albumIdParam.Value = entry.AlbumId;
                            updated.Value = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
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
        /// Retrieves a list of all the available AlbumEntries associated with covers.
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns>List{AlbumEntry}.</returns>
        public List<AlbumEntry> GetCoverHashes(int limit, int offset)
        {
            var data = new List<AlbumEntry>();
            try
            {
                using (var mConnection = new SQLiteConnection(_dbConnection))
                using (var mCommand = new SQLiteCommand(mConnection))
                {
                    mConnection.Open();
                    mCommand.CommandText = "select * from covers limit @limit offset @offset";
                    var limitParam = mCommand.CreateParameter();
                    var offsetParam = mCommand.CreateParameter();
                    limitParam.ParameterName = "@limit";
                    offsetParam.ParameterName = "@offset";
                    limitParam.Value = limit;
                    offsetParam.Value = offset;
                    mCommand.Parameters.Add(limitParam);
                    mCommand.Parameters.Add(offsetParam);
                    var mReader = mCommand.ExecuteReader();
                    while (mReader.Read())
                    {
                        var entry = new AlbumEntry(mReader["album_id"].ToString())
                        {
                            CoverHash = mReader["coverhash"].ToString()
                        };

                        data.Add(entry);
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
