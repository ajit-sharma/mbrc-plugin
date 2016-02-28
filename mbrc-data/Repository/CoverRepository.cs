namespace MusicBeeRemoteData.Repository
{
    using System;
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class CoverRepository : ICoverRepository
    {
        public int Delete(IList<LibraryCover> Covers)
        {
            throw new NotImplementedException();
        }

        public int SoftDelete(IList<LibraryCover> t)
        {
            throw new NotImplementedException();
        }

        public IList<LibraryCover> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<LibraryCover> GetCached()
        {
            throw new NotImplementedException();
        }

        public LibraryCover GetById(long id)
        {
            throw new NotImplementedException();
        }

        public int GetCount()
        {
            throw new NotImplementedException();
        }

        public IList<LibraryCover> GetPage(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public IList<LibraryCover> GetDeleted()
        {
            throw new NotImplementedException();
        }

        public IList<LibraryCover> GetUpdatedPage(int offset, int limit, long epoch)
        {
            throw new NotImplementedException();
        }

        public int Save(LibraryCover Cover)
        {
            throw new NotImplementedException();
        }

        public int Save(IList<LibraryCover> t)
        {
            throw new NotImplementedException();
        }
    }
}