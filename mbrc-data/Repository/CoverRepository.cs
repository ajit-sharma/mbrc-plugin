namespace MusicBeeRemoteData.Repository
{
    using System;
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;

    public class CoverRepository : ICoverRepository
    {
        public void DeleteCovers(ICollection<LibraryCover> Covers)
        {
            throw new NotImplementedException();
        }

        public ICollection<LibraryCover> GetAllCovers()
        {
            throw new NotImplementedException();
        }

        public ICollection<LibraryCover> GetCachedCovers()
        {
            throw new NotImplementedException();
        }

        public LibraryCover GetCover(long id)
        {
            throw new NotImplementedException();
        }

        public int GetCoverCount()
        {
            throw new NotImplementedException();
        }

        public ICollection<LibraryCover> GetCoverPage(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public ICollection<LibraryCover> GetDeletedCovers()
        {
            throw new NotImplementedException();
        }

        public ICollection<LibraryCover> GetUpdatedCovers(int offset, int limit, long epoch)
        {
            throw new NotImplementedException();
        }

        public int SaveCover(LibraryCover Cover)
        {
            throw new NotImplementedException();
        }

        public void SaveCovers(ICollection<LibraryCover> Covers)
        {
            throw new NotImplementedException();
        }
    }
}