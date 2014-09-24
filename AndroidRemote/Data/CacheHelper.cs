using System.Data;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.OrmLite;

namespace MusicBeePlugin.AndroidRemote.Data
{
    using NLog;
    using System;

    /// <summary>
    /// Class CacheHelper.
    /// Is used to handle the library data and cover cache
    /// </summary>
    public class CacheHelper
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
                    db.CreateTableIfNotExists<LibraryCover>();
                    db.CreateTableIfNotExists<LibraryArtist>();
                    db.CreateTableIfNotExists<LibraryAlbum>();
                    db.CreateTableIfNotExists<LibraryGenre>();
                    db.CreateTableIfNotExists<LibraryTrack>();
                    db.CreateTableIfNotExists<Playlist>();    
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
    }
}
