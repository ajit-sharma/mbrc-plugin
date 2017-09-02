using MusicBeeRemote.Data.Entities;

namespace MusicBeeRemote.Data.Repository.Interfaces
{
    /// <summary>
    /// The PlaylistRepository interface.
    /// The repository should contain only playlist specific repository methods.
    /// The generic repository methods should be in the <see cref="IRepository{T}"/>.
    /// </summary>
    public interface IPlaylistRepository : IRepository<Playlist>
    {
    }
}