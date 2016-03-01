namespace MusicBeeRemoteData.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Extensions;
    using MusicBeeRemoteData.Repository.Interfaces;

    using NUnit.Framework;

    using Ploeh.AutoFixture;

    [TestFixture]
    public class ArtistRepositoryTests
    {

        [SetUp]
        public void Setup()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            this.directory = Path.GetDirectoryName(path);
            this.fixture = new Fixture();
        }

        [TearDown]
        public void Cleanup()
        {
            this.databaseProvider.DeleteDatabase();
        }

        private string directory;

        private Fixture fixture;

        private DatabaseProvider databaseProvider;

        
        private IArtistRepository Repository()
        {
            this.databaseProvider = new DatabaseProvider(this.directory);
            this.databaseProvider.ResetDatabase();
            IArtistRepository repository = new ArtistRepository(this.databaseProvider);
            return repository;
        }

        private IList<LibraryArtist> GenerateArtists(int count)
        {
            var epoch = DateTime.UtcNow.ToUnixTime();
            return
                this.fixture.Build<LibraryArtist>()
                    .With(t => t.DateAdded, epoch)
                    .Without(t => t.DateUpdated)
                    .Without(t => t.DateDeleted)
                    .Without(t => t.Id)
                    .CreateMany(count)
                    .ToList();
        }

        private LibraryArtist GenerateArtist()
        {
            var epoch = DateTime.UtcNow.ToUnixTime();
            return
                this.fixture.Build<LibraryArtist>()
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
            const int Number = 53;
            var repository = this.Repository();
            var artists = this.GenerateArtists(Number);
            var saved = repository.Save(artists);
            Assert.AreEqual(Number, saved);
            var all = repository.GetAll();
            Assert.AreEqual(Number, all.Count);
            Assert.AreEqual(artists[0].Name, all[0].Name);         

        }
    }
}