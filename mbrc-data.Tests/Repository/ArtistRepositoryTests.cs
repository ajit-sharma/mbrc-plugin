using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MusicBeeRemoteData;
using MusicBeeRemoteData.Entities;
using MusicBeeRemoteData.Extensions;
using MusicBeeRemoteData.Repository;
using MusicBeeRemoteData.Repository.Interfaces;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace mbrc_data.Tests.Repository
{
    [TestFixture]
    public class ArtistRepositoryTests
    {
       
        private string _directory;

        private Fixture _fixture;

        private DatabaseProvider _databaseProvider;
        
        [SetUp]
        public void Setup()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            _directory = Path.GetDirectoryName(path);
            _fixture = new Fixture();
        }

        [TearDown]
        public void Cleanup()
        {
            _databaseProvider.DeleteDatabase();
        }


        
        private IArtistRepository Repository()
        {
            _databaseProvider = new DatabaseProvider(_directory);
            _databaseProvider.ResetDatabase();
            IArtistRepository repository = new ArtistRepository(_databaseProvider);
            return repository;
        }

        private IList<ArtistDao> GenerateArtists(int count)
        {
            var epoch = DateTime.UtcNow.ToUnixTime();
            return
                _fixture.Build<ArtistDao>()
                    .With(t => t.DateAdded, epoch)
                    .Without(t => t.DateUpdated)
                    .Without(t => t.DateDeleted)
                    .Without(t => t.Id)
                    .CreateMany(count)
                    .ToList();
        }

        private ArtistDao GenerateArtist()
        {
            var epoch = DateTime.UtcNow.ToUnixTime();
            return
                _fixture.Build<ArtistDao>()
                    .With(t => t.DateAdded, epoch)
                    .Without(t => t.DateUpdated)
                    .Without(t => t.DateDeleted)
                    .Without(t => t.Id)
                    .Create();
        }


        [Test]
        public void ArtistRepositoryTest()
        {
            Assert.Fail();
        }

        [Test]
        public void TestSavingData()
        {
            const int totalTracks = 53;
            var repository = Repository();
            var artists = GenerateArtists(totalTracks);
            var saved = repository.Save(artists);
            Assert.AreEqual(totalTracks, saved);
            var all = repository.GetAll();
            Assert.AreEqual(totalTracks, all.Count);
            Assert.AreEqual(artists[0].Name, all[0].Name);         

        }
    }
}