#region

using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    [Route("/playlists", "GET")]
    public class AllPlaylists : IReturn<PaginatedResult>
    {
        public int offset { get; set; }
        public int limit { get; set; }
    }

    [Route("/playlists", "PUT")]
    public class CreatePlaylist
    {
        public string name { get; set; }
        public string[] list { get; set; }
    }

    [Route("/playlists/play", "PUT")]
    public class PlaylistPlay
    {
        public string path { get; set; }
    }

    [Route("/playlists/{id}", "GET")]
    public class GetPlaylist
    {
        public int id { get; set; }
    }

    [Route("/playlists/{id}", "DELETE")]
    public class DeletePlaylist
    {
        public int id { get; set; }
    }

    [Route("/playlists/{id}/tracks", "GET")]
    public class GetPlaylistTracks
    {
        public int id { get; set; }
    }

    [Route("/playlist/{id}/tracks", "PUT")]
    public class AddPlaylistTracks
    {
        public string[] list { get; set; }
    }

    [Route("/playlist/{id}/tracks", "DELETE")]
    public class DeletePlaylistTracks
    {
        public int index { get; set; }
    }
}