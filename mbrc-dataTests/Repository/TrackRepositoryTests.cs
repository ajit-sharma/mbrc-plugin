namespace MusicBeeRemoteData.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using MusicBeeRemoteData.Entities;

    using NUnit.Framework;

    using Ploeh.AutoFixture;

    [TestFixture()]
    public class TrackRepositoryTests
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
        }

        private string directory;

        private Fixture fixture;

        /// <summary>
        /// Creates and returns a new <see cref="ITrackRepository"/> with a clean
        /// database.
        /// </summary>
        /// <returns>
        /// The <see cref="ITrackRepository"/>.
        /// </returns>
        private ITrackRepository TrackRepository()
        {
            var provider = new DatabaseProvider(this.directory);
            provider.ResetDatabase();
            ITrackRepository repository = new TrackRepository(provider);
            return repository;
        }

        private List<LibraryTrack> GenerateTracks(int count)
        {
            return this.fixture.Build<LibraryTrack>().Without(t => t.Id).CreateMany(count).ToList();
        }

        [Test(Description = "Delete all valid id tracks")]
        public void DeleteTracksTest()
        {
            var repository = this.TrackRepository();
            var libraryTracks = this.GenerateTracks(100);
            var affectedRows = repository.SaveTracks(libraryTracks);
            Assert.AreEqual(100, affectedRows);

            var tracks = repository.GetAllTracks();
            Assert.AreEqual(100, tracks.Count);
            var deletedRows = repository.DeleteTracks(tracks);
            Assert.AreEqual(100, deletedRows);
            var allTracks = repository.GetAllTracks();
            Assert.AreEqual(0, allTracks.Count);
        }

        [Test(Description = "Some of the ids included should be invalid")]
        public void DeleteTracksInvalid()
        {
            var repository = this.TrackRepository();
            var libraryTracks = this.GenerateTracks(100);
            var affectedRows = repository.SaveTracks(libraryTracks);
            Assert.AreEqual(100, affectedRows);

            var tracks = repository.GetAllTracks();
            Assert.AreEqual(100, tracks.Count);

            tracks.Add(new LibraryTrack
                           {
                               Id = 19321,
                           });
            tracks.Add(new LibraryTrack
                           {
                               Id = 1249
                           });

            tracks = tracks.Reverse().ToList();

            var deletedRows = repository.DeleteTracks(tracks);
            Assert.AreEqual(100, deletedRows);
            var allTracks = repository.GetAllTracks();
            Assert.AreEqual(0, allTracks.Count);
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
            var repository = this.TrackRepository();
            var count = 50;
            var libraryTracks = this.GenerateTracks(count);

            libraryTracks.Take(20).ToList().ForEach(t => t.AlbumId = 10);

            var saveTracks = repository.SaveTracks(libraryTracks);
            Assert.AreEqual(saveTracks, libraryTracks.Count);
            var albumTracks = repository.GetTracksByAlbumId(10);
            Assert.GreaterOrEqual(albumTracks.Count, 20);
        }

        [Test()]
        public void GetTrackTest()
        {
            var repository = this.TrackRepository();

            var track = this.fixture.Build<LibraryTrack>().Without(t => t.Id).Create();

            var id = repository.SaveTrack(track);
            var libraryTrack = repository.GetTrack(id);

            Assert.AreEqual(id, libraryTrack.Id);
            Assert.AreEqual(track.Path, libraryTrack.Path);
        }

        [Test()]
        public void GetUpdatedTracksTest()
        {
            var repository = this.TrackRepository();

            var libraryTracks =
                this.fixture.Build<LibraryTrack>().Without(t => t.Id).With(track => track.DateUpdated).CreateMany(50);

            var average = libraryTracks.Select(track => track.DateUpdated).Average();
            var count = libraryTracks.Select(track => track.DateUpdated).Count(date => date > average);

            var tracklist = libraryTracks.ToList();
            repository.SaveTracks(tracklist);

            var updatedTracks = repository.GetUpdatedTracks(0, count, (long)average);

            Assert.AreEqual(count, updatedTracks.Count);
        }

        /// <summary>
        /// The save tracks test.
        /// </summary>
        [Test(Description = "Testing save functionality")]
        public void SaveTracksTest()
        {
            var repository = this.TrackRepository();

            var libraryTracks = this.fixture.Build<LibraryTrack>().Without(t => t.Id).CreateMany(50);

            var tracklist = libraryTracks.ToList();

            var tracks = repository.SaveTracks(tracklist);
            Assert.AreEqual(tracklist.Count, tracks);
        }

        [Test()]
        public void SaveTrackTest()
        {
            var repository = this.TrackRepository();

            var track = this.fixture.Build<LibraryTrack>().Without(t => t.Id).Create();

            var id = repository.SaveTrack(track);
            Assert.AreNotEqual(0, id);
        }

        [Test()]
        public void TrackRepositoryTest()
        {
            Assert.Fail();
        }
    }
}