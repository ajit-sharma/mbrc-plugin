#region

using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    [Route("/playlists", "GET")]
    public class AllPlaylists : IReturn<PaginatedResponse>
    {
        public int offset { get; set; }
        public int limit { get; set; }
    }

    [Route("/playlists", "PUT")]
    public class CreatePlaylist : IReturn<SuccessResponse>
    {
        public string name { get; set; }
        public string[] list { get; set; }
    }

    [Route("/playlists/play", "PUT")]
    public class PlaylistPlay : IReturn<SuccessResponse>
    {
        public string path { get; set; }
    }

    [Route("/playlists/{id}", "GET")]
    public class GetPlaylist
    {
        public int id { get; set; }
    }

    [Route("/playlists/{id}", "DELETE")]
    public class DeletePlaylist : IReturn<SuccessResponse>
    {
        public int id { get; set; }
    }

    [Route("/playlists/{id}/tracks", "GET")]
    public class GetPlaylistTracks : IReturn<PaginatedResponse>
    {
        public int id { get; set; }
    }

    [Route("/playlists/{id}/tracks", "PUT")]
    public class AddPlaylistTracks : IReturn<SuccessResponse>
    {
        public string[] list { get; set; }
        public int id { get; set; }
    }

    [Route("/playlists/{id}/tracks", "DELETE")]
    public class DeletePlaylistTracks : IReturn<SuccessResponse>
    {
        public int index { get; set; }
        public int id { get; set; }
    }

    public class SuccessResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
        public bool success { get; set; }
    }
}