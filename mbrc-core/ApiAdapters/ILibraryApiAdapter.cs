namespace MusicBeeRemoteCore
{
    using System;
    using System.Collections.Generic;

    using MusicBeeRemoteCore.Model;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using MusicBeeRemoteData.Entities;

    public interface ILibraryApiAdapter
    {
        ICollection<LibraryAlbum> GetAlbumList();

        ICollection<LibraryArtist> GetArtistList();

        string GetArtistUrl(string name);

        byte[] GetCoverData(string path);

        string GetCoverUrl(string path);

        ICollection<LibraryGenre> GetGenreList();

        string[] GetLibraryFiles();

        Modifications GetSyncDelta(string[] cachedFiles, DateTime lastSync);

        LibraryTrackEx GetTags(string file);
    }
}