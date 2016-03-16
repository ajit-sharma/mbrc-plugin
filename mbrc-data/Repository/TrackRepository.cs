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

        public string GetFirstAlbumTrackPathById(long id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var path =
                    connection.Query<string>(
                        $@"select LibraryTrack.Path
                            from LibraryTrack
                            where LibraryTrack.AlbumId = {
                            id
                            }
                            order by LibraryTrack.Disc, LibraryTrack.Position asc
                            limit 1");

                connection.Close();
                return path.FirstOrDefault();
            }
        }

        public string[] GetTrackPathsByArtistId(long id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var paths =
                    connection.Query<string>(
                        $@"select LibraryTrack.Path
                                                    from LibraryTrack 
                                                    inner join LibraryAlbum
                                                    on LibraryAlbum.Id = LibraryTrack.AlbumId
                                                    where LibraryTrack.ArtistId = {
                            id
                            }
                                                    order By LibraryAlbum.Name, LibraryTrack.Disc, LibraryTrack.Position asc")
                        .ToArray();
                connection.Close();
                return paths;
            }
        }

        public string[] GetTrackPathsByGenreId(long id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var sql = $@"select track.Path 
                            from LibraryTrack as track
                            inner join LibraryAlbum album
                            on album.Id = track.AlbumId
                            inner join LibraryArtist as artist
                            on artist.Id = track.ArtistId
                            left join LibraryArtist as albumArtist
                            on albumArtist.Id = track.AlbumArtistId
                            where track.GenreId = {id}
                            order by albumArtist.Name, album.Name, track.Disc, track.Position";

                var paths = connection.Query<string>(sql);
                connection.Close();
                return paths.ToArray();
            }
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