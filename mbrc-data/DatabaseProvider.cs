namespace MusicBeeRemoteData
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.IO;

    using Dapper;

    using NLog;

    /// <summary>
    ///     Class DatabaseProvider.
    ///     Is used to handle the library data and cover cache
    /// </summary>
    public class DatabaseProvider
    {
        private const string DbName = @"\\cache.db";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _dbFilePath;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseProvider" /> class.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        public DatabaseProvider(string storagePath)
        {
            this._dbFilePath = storagePath + DbName;

            try
            {
                this.CreateDatabase();
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }
        }

        /// <summary>
        ///     Gets a connection to the database.
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetDbConnection()
        {
            return new SQLiteConnection($"Data Source={this._dbFilePath}");
        }

        private void CreateDatabase()
        {
            if (File.Exists(this._dbFilePath))
            {
                return;
            }

            using (var connection = this.GetDbConnection())
            {
                connection.Open();
                connection.Execute(@"create table LibraryGenre (
                      Id integer primary key AUTOINCREMENT,
                      Name varchar(255) not null,
                      DateAdded integer default (strftime('%s', 'now')),
                      DateUpdated integer default 0,
                      DateDeleted integer default 0
                    );");

                connection.Execute(@"create unique index uidx_librarygenre_name on LibraryGenre (Name);");

                connection.Execute(@"create table LibraryArtist (
                        Id integer primary key AUTOINCREMENT,
                        Name varchar(255) not null,
                        GenreId integer not null,
                        ImageUrl varchar(255),
                        DateAdded integer default (strftime('%s', 'now')),
                        DateUpdated integer default 0,
                        DateDeleted integer default 0,
                        foreign key (GenreId) references LibraryGenre (Id) DEFERRABLE INITIALLY DEFERRED
                        );");

                connection.Execute(@"create unique index uidx_libraryartist_name on LibraryArtist (Name);");
                connection.Execute(@"create table LibraryAlbum
                    (
                        Id integer primary key AUTOINCREMENT,
                        Name varchar(255),
                        ArtistId  integer not null,
                        CoverId  integer not null,
                        AlbumId varchar(255),
                        DateAdded integer default (strftime('%s', 'now')),
                        DateUpdated integer default 0,
                        DateDeleted integer default 0,
                        foreign key (ArtistId) references LibraryArtist (Id) DEFERRABLE INITIALLY DEFERRED,
                        foreign key (CoverId) references LibraryCover (Id) DEFERRABLE INITIALLY DEFERRED
                    );");

                connection.Execute(@"create unique index uidx_libraryalbum_name on LibraryAlbum (Name);");

                connection.Execute(@"create table LibraryCover (
                        Id integer primary key AUTOINCREMENT,
                        Hash varchar(40),
                        DateAdded integer default (strftime('%s', 'now')),
                        DateUpdated integer default 0,
                        DateDeleted integer default 0
                    );");

                connection.Execute(@"create table LibraryTrack (
                        Id integer primary key AUTOINCREMENT,
                        Title varchar(255),
                        Position integer default 0,
                        Disc integer default 0,
                        GenreId integer not null,
                        ArtistId integer not null,
                        AlbumArtistId integer not null,
                        AlbumId integer not null,
                        Year varchar(40),
                        Path varchar(255),
                        DateAdded integer default (strftime('%s', 'now')),
                        DateUpdated integer default 0,
                        DateDeleted integer default 0,
                        foreign key (AlbumId) references LibraryAlbum (Id) DEFERRABLE INITIALLY DEFERRED,
                        foreign key (AlbumArtistId) references LibraryArtist (Id) DEFERRABLE INITIALLY DEFERRED,
                        foreign key (ArtistId) references LibraryArtist (Id) DEFERRABLE INITIALLY DEFERRED,
                        foreign key (GenreId) references LibraryGenre (Id) DEFERRABLE INITIALLY DEFERRED
                    );");

                connection.Execute(@"create unique index uidx_librarytrack_title on LibraryTrack (Title);");

                connection.Execute(@"create table Playlist (
                        Id integer primary key AUTOINCREMENT,
                        Name varchar(255),
                        Tracks integer default 0,
                        ReadOnly integer default 1,
                        Path varchar(255),
                        DateAdded integer default (strftime('%s', 'now')),
                        DateUpdated integer default 0,
                        DateDeleted integer default 0
                    );");

                connection.Execute(@"create table PlaylistTrackInfo (
                        Id integer primary key AUTOINCREMENT,
                        Path varchar (255),
                        Artist varchar (255),
                        Title varchar(255),
                        DateAdded integer default (strftime('%s', 'now')),
                        DateUpdated integer default 0,
                        DateDeleted integer default 0
                    );");

                connection.Execute(@"create table PlaylistTrack (
                        Id integer primary key AUTOINCREMENT,
                        TrackInfoId integer not null,
                        PlaylistId integer not null,
                        Position integer not null,
                        DateAdded integer default (strftime('%s', 'now')),
                        DateUpdated integer default 0,
                        DateDeleted integer default 0,
                        foreign key (PlaylistId) references Playlist(Id) DEFERRABLE INITIALLY DEFERRED,
                        foreign key (TrackInfoId) references PlaylistTrackInfo (Id) DEFERRABLE INITIALLY DEFERRED
                    );");
            }
        }
    }
}