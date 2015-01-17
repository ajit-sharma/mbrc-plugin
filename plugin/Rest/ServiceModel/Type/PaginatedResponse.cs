#region

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public abstract class PaginatedResponse<T>
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }

        [DataMember(Name = "limit")]
        public int Limit { get; set; }

        [DataMember(Name = "offset")]
        public int Offset { get; set; }

        [DataMember(Name = "data")]
        public virtual List<T> Data { get; set; }

        public void CreatePage(int limit, int offset, List<T> data)
        {
            Offset = offset;
            Limit = limit;
            Total = data.Count;

            if (offset == 0 && limit == 0) return;

            var range = offset + limit;
            var size = data.Count;
            if (range <= size)
            {
                data = data.GetRange(offset, limit);
                Data = data;
            }
            else if (offset < size)
            {
                limit = size - offset;
                data = data.GetRange(offset, limit);
                Data = data;
                Limit = limit;
            }
        }
    }

    public class PaginatedArtistResponse : PaginatedResponse<LibraryArtist>
    {
        [DataMember(Name = "data")]
        public override List<LibraryArtist> Data { get; set; }
    }

    public class PaginatedTrackResponse : PaginatedResponse<LibraryTrack>
    {
        [DataMember(Name = "data")]
        public override List<LibraryTrack> Data { get; set; }
    }

    public class PaginatedGenreResponse : PaginatedResponse<LibraryGenre>
    {
        [DataMember(Name = "data")]
        public override List<LibraryGenre> Data { get; set; }
    }

    public class PaginatedAlbumResponse : PaginatedResponse<LibraryAlbum>
    {
        [DataMember(Name = "data")]
        public override List<LibraryAlbum> Data { get; set; }
    }
    public class PaginatedCoverResponse : PaginatedResponse<LibraryCover>
    {
        [DataMember(Name = "data")]
        public override List<LibraryCover> Data { get; set; }
    }

    public class PaginatedNowPlayingResponse : PaginatedResponse<NowPlaying>
    {
        [DataMember(Name = "data")]
        public override List<NowPlaying> Data { get; set; }
    }

    public class PaginatedPlaylistResponse : PaginatedResponse<Playlist>
    {
        [DataMember(Name = "data")]
        public override List<Playlist> Data { get; set; }
    }

    public class PaginatedPlaylistTrackResponse : PaginatedResponse<PlaylistTrack>
    {
        [DataMember(Name = "data")]
        public override List<PlaylistTrack> Data { get; set; }
    }

}