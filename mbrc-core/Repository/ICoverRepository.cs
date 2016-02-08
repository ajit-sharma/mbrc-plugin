namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface ICoverRepository
    {
        void DeleteCovers(ICollection<LibraryCover> Covers);

        ICollection<LibraryCover> GetAllCovers();

        ICollection<LibraryCover> GetCachedCovers();

        LibraryCover GetCover(long id);

        int GetCoverCount();

        ICollection<LibraryCover> GetCoverPage(int offset, int limit);

        ICollection<LibraryCover> GetDeletedCovers();

        ICollection<LibraryCover> GetUpdatedCovers(int offset, int limit, long epoch);

        int SaveCover(LibraryCover Cover);

        void SaveCovers(ICollection<LibraryCover> Covers);
    }
}