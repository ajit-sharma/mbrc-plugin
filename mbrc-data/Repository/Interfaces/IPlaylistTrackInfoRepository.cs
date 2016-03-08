using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace MusicBeeRemoteData.Repository.Interfaces
{
    using MusicBeeRemoteData.Entities;

    /// <summary>
    /// The PlaylistTrackInfoRepository interface.
    /// The repository should contain only playlist track info specific repository methods.
    /// The generic repository methods should be in the <see cref="IRepository{T}"/>.
    /// </summary>
    public interface IPlaylistTrackInfoRepository : IRepository<PlaylistTrackInfo>
    {
      IList<PlaylistTrackInfo> GetTrackForPlaylist(int id);
    }
}