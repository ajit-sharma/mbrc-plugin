namespace MusicBeePlugin.Repository
{
    using System;
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    class CoverRepository : ICoverRepository
    {
        public LibraryCover GetCover(long id)
        {
            throw new NotImplementedException();
        }

        public void SaveCover(LibraryCover Cover)
        {
            throw new NotImplementedException();
        }

        public void SaveCovers(IEnumerable<LibraryCover> Covers)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LibraryCover> GetAllCovers()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LibraryCover> GetCoverPage(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LibraryCover> GetUpdatedCovers(int offset, int limit, long epoch)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LibraryCover> GetCachedCovers()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LibraryCover> GetDeletedCovers()
        {
            throw new NotImplementedException();
        }

        public void DeleteCovers(IEnumerable<LibraryCover> Covers)
        {
            throw new NotImplementedException();
        }

        public int GetCoverCount()
        {
            throw new NotImplementedException();
        }
    }
}