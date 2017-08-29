using System.Collections.Generic;
using System.Runtime.Serialization;
using MusicBeeRemoteData.Entities;

namespace MusicBeeRemote.Core.Rest.ServiceModel.Type
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public abstract class PaginatedResponse<T> : ResponseBase
    {
        [DataMember(Name = "data")]
        public virtual List<T> Data { get; set; }

        [DataMember(Name = "limit")]
        public int Limit { get; set; }

        [DataMember(Name = "offset")]
        public int Offset { get; set; }

        [DataMember(Name = "total")]
        public int Total { get; set; }

        public void CreatePage(int limit, int offset, List<T> data)
        {
            this.Offset = offset;
            this.Limit = limit;
            this.Total = data.Count;
            this.Data = data;

            if (offset == 0 && limit == 0)
            {
                return;
            }

            var range = offset + limit;
            var size = data.Count;
            if (range <= size)
            {
                data = data.GetRange(offset, limit);
                this.Data = data;
            }
            else if (offset < size)
            {
                limit = size - offset;
                data = data.GetRange(offset, limit);
                this.Data = data;
                this.Limit = limit;
            }
            else
            {
                this.Data = new List<T>();
            }
        }
    }

    [DataContract]
    public class PaginatedArtistResponse : PaginatedResponse<ArtistDao>
    {
        [DataMember(Name = "data")]
        public override List<ArtistDao> Data { get; set; }
    }

    [DataContract]
    public class PaginatedTrackResponse : PaginatedResponse<TrackDao>
    {
        [DataMember(Name = "data")]
        public override List<TrackDao> Data { get; set; }
    }

    [DataContract]
    public class PaginatedGenreResponse : PaginatedResponse<GenreDao>
    {
        [DataMember(Name = "data")]
        public override List<GenreDao> Data { get; set; }
    }

    [DataContract]
    public class PaginatedAlbumResponse : PaginatedResponse<AlbumDao>
    {
        [DataMember(Name = "data")]
        public override List<AlbumDao> Data { get; set; }
    }

    [DataContract]
    public class PaginatedCoverResponse : PaginatedResponse<LibraryCover>
    {
        [DataMember(Name = "data")]
        public override List<LibraryCover> Data { get; set; }
    }

    [DataContract]
    public class PaginatedNowPlayingResponse : PaginatedResponse<NowPlaying>
    {
        [DataMember(Name = "data")]
        public override List<NowPlaying> Data { get; set; }
    }

    [DataContract]
    public class PaginatedPlaylistResponse : PaginatedResponse<Playlist>
    {
        [DataMember(Name = "data")]
        public override List<Playlist> Data { get; set; }
    }

    [DataContract]
    public class PaginatedPlaylistTrackInfoResponse : PaginatedResponse<PlaylistTrackInfo>
    {
        [DataMember(Name = "data")]
        public override List<PlaylistTrackInfo> Data { get; set; }
    }

    [DataContract]
    public class PaginatedPlaylistTrackResponse : PaginatedResponse<PlaylistTrack>
    {
        [DataMember(Name = "data")]
        public override List<PlaylistTrack> Data { get; set; }
    }
}