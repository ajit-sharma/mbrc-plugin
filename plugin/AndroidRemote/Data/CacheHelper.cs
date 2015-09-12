using MusicBeePlugin.Rest.ServiceModel.Type;
using System.Data;
using MusicBeePlugin.AndroidRemote.Persistence;
using NServiceKit.OrmLite;

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
					db.CreateTableIfNotExists<PlaylistTrackInfo>();
					db.CreateTableIfNotExists<PlaylistTrack>();
					db.CreateTableIfNotExists<LastUpdated>();
                }

            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }
        }

	    /// <summary>
		/// Gets a connection to the database.
	    /// </summary>
	    /// <returns></returns>
	    public IDbConnection GetDbConnection()
        {
            return _dbConnection.OpenDbConnection();
        }
    }
}
