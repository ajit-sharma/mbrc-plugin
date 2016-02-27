namespace MusicBeeRemoteData.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using MusicBeeRemoteData.Entities;

    using NUnit.Framework;

    using Ploeh.AutoFixture;

    [TestFixture()]
    public class TrackRepositoryTests
    {
        private string directory;

        private Fixture fixture;

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
        }

        [Test()]
        public void DeleteTracksTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetAllTracksTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetCachedTracksTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetDeletedTracksTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetTrackCountTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetTrackPageTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetTracksByAlbumIdTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetTrackTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetUpdatedTracksTest()
        {
            Assert.Fail();
        }

        /// <summary>
        /// The save tracks test.
        /// </summary>
        [Test(Description = "Testing save functionality")]
        public void SaveTracksTest()
        {
            var provider = new DatabaseProvider(this.directory);
            provider.ResetDatabase();
            ITrackRepository repository = new TrackRepository(provider);            
            var tracklist = new List<LibraryTrack>();
            this.fixture.AddManyTo(tracklist, 50);
            var tracks = repository.SaveTracks(tracklist);
            Assert.AreEqual(tracklist.Count, tracks);
        }

        [Test()]
        public void SaveTrackTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void TrackRepositoryTest()
        {
            Assert.Fail();
        }
    }    
}

