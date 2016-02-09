namespace MusicBeePlugin
{
    using System;
    using System.Collections.Generic;

    using MusicBeePlugin.Model;
    using MusicBeePlugin.Rest.ServiceModel.Type;

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