using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Repository
{
    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface ICoverRepository
    {
        LibraryCover GetCover(long id);

        void SaveCover(LibraryCover Cover);

        void SaveCovers(IEnumerable<LibraryCover> Covers);

        IEnumerable<LibraryCover> GetAllCovers();

        IEnumerable<LibraryCover> GetCoverPage(int offset, int limit);

        IEnumerable<LibraryCover> GetUpdatedCovers(int offset, int limit, long epoch);

        IEnumerable<LibraryCover> GetCachedCovers();

        IEnumerable<LibraryCover> GetDeletedCovers();

        void DeleteCovers(IEnumerable<LibraryCover> Covers);

        int GetCoverCount();
    }
}
