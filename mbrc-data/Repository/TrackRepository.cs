namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;
    using System.Linq;

    using Dapper;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    /// <summary>
    /// The track repository, gives access to all the track data in the plugin's cache.
    /// </summary>
    public class TrackRepository : GenericRepository<LibraryTrack>, ITrackRepository
    {
        public TrackRepository(DatabaseProvider provider)
            : base(provider)
        {
        }

        public string[] GetTrackPathsByArtistId(long id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var paths = connection.Query<string>($@"select LibraryTrack.Path
                                                    from LibraryTrack 
                                                    inner join LibraryAlbum
                                                    on LibraryAlbum.Id = LibraryTrack.AlbumId
                                                    where LibraryTrack.ArtistId = {id}
                                                    order By LibraryAlbum.Name, LibraryTrack.Disc, LibraryTrack.Position asc").ToArray();
                connection.Close();
                return paths;
            }
        }

        public string[] GetTrackPathsByGenreId(long id)
        {
            var sql = $@"select LibraryTrack.Path
                        from LibraryTrack
                        inner join LibraryArtist
                        on LibraryArtist.Id = LibraryTrack.AlbumArtistId
                        inner join LibraryAlbum 
                        on LibraryAlbum.Id = LibraryTrack.AlbumId
                        where LibraryTrack.GenreId = {id}
                        order by LibraryArtist.Name, LibraryAlbum.Name, LibraryTrack.Disc, LibraryTrack.Position asc";

            throw new System.NotImplementedException();

        }

        public IList<LibraryTrack> GetTracksByAlbumId(long id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var tracks = connection.GetList<LibraryTrack>($"where AlbumId = {id}");
                connection.Close();
                return tracks.ToList();
            }
        }
    }
}