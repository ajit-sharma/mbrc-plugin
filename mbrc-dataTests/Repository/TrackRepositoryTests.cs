namespace MusicBeeRemoteData.Repository.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Extensions;

    using NUnit.Framework;

    using Ploeh.AutoFixture;

    [TestFixture(Description = "Test the track repository functionality")]
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
            this.databaseProvider.DeleteDatabase();
        }

        private string directory;

        private Fixture fixture;

        private DatabaseProvider databaseProvider;

        /// <summary>
        /// Creates and returns a new <see cref="ITrackRepository"/> with a clean
        /// database.
        /// </summary>
        /// <returns>
        /// The <see cref="ITrackRepository"/>.
        /// </returns>
        private ITrackRepository TrackRepository()
        {
            this.databaseProvider = new DatabaseProvider(this.directory);
            this.databaseProvider.ResetDatabase();
            ITrackRepository repository = new TrackRepository(this.databaseProvider);
            return repository;
        }

        private List<LibraryTrack> GenerateTracks(int count)
        {
            var epoch = DateTime.UtcNow.ToUnixTime();
            return
                this.fixture.Build<LibraryTrack>()
                    .With(t => t.DateAdded, epoch)
                    .Without(t => t.DateUpdated)
                    .Without(t => t.DateDeleted)
                    .Without(t => t.Id)
                    .CreateMany(count)
                    .ToList();
        }

        private LibraryTrack GenerateTrack()
        {
            var epoch = DateTime.UtcNow.ToUnixTime();
            return
                this.fixture.Build<LibraryTrack>()
                    .With(t => t.DateAdded, epoch)
                    .Without(t => t.DateUpdated)
                    .Without(t => t.DateDeleted)
                    .Without(t => t.Id)
                    .Create();
        }

        [Test(Description = "Some of the ids included should be invalid")]
        public void DeleteTracksInvalid()
        {
            var repository = this.TrackRepository();
            var libraryTracks = this.GenerateTracks(100);
            var affectedRows = repository.Save(libraryTracks);
            Assert.AreEqual(100, affectedRows);

            var tracks = repository.GetAll();
            Assert.AreEqual(100, tracks.Count);

            tracks.Add(new LibraryTrack { Id = 19321, });
            tracks.Add(new LibraryTrack { Id = 1249 });

            tracks = tracks.Reverse().ToList();

            var deletedRows = repository.Delete(tracks);
            Assert.AreEqual(100, deletedRows);
            var allTracks = repository.GetAll();
            Assert.AreEqual(0, allTracks.Count);
        }

        [Test(Description = "Delete all valid id tracks")]
        public void DeleteTracksTest()
        {
            var repository = this.TrackRepository();
            var libraryTracks = this.GenerateTracks(100);
            var affectedRows = repository.Save(libraryTracks);
            Assert.AreEqual(100, affectedRows);

            var tracks = repository.GetAll();
            Assert.AreEqual(100, tracks.Count);
            var deletedRows = repository.Delete(tracks);
            Assert.AreEqual(100, deletedRows);
            var allTracks = repository.GetAll();
            Assert.AreEqual(0, allTracks.Count);
        }

        [Test(Description = "Test the retrieval of all the data")]
        public void GetAllTracksTest()
        {
            const int TrackNum = 156;
            const int Deleted = 50;
            var repository = this.TrackRepository();
            var tracks = this.GenerateTracks(TrackNum);
            var epoch = DateTime.UtcNow.ToUnixTime();
            tracks.Take(Deleted).ToList().ForEach(track => { track.DateDeleted = epoch; });
            repository.Save(tracks);
            var all = repository.GetAll();
            Assert.AreEqual(TrackNum, all.Count);
            var deleted = repository.GetDeleted();
            Assert.AreEqual(Deleted, deleted.Count);
        }

        [Test(Description = "Test the retrieval of cached")]
        public void GetCachedTracksTest()
        {
            const int TrackNum = 156;
            const int Deleted = 50;
            var repository = this.TrackRepository();
            var tracks = this.GenerateTracks(TrackNum);
            var epoch = DateTime.UtcNow.ToUnixTime();
            tracks.Take(Deleted).ToList().ForEach(track => { track.DateDeleted = epoch; });
            repository.Save(tracks);

            var cached = repository.GetCached();
            Assert.AreEqual(TrackNum - Deleted, cached.Count);
        }

        [Test()]
        public void GetDeletedTracksTest()
        {
            const int TrackNum = 156;
            const int Deleted = 50;
            var repository = this.TrackRepository();
            var tracks = this.GenerateTracks(TrackNum);
            var epoch = DateTime.UtcNow.ToUnixTime();
            tracks.Take(Deleted).ToList().ForEach(track => { track.DateDeleted = epoch; });
            repository.Save(tracks);

            var deleted = repository.GetDeleted();
            Assert.AreEqual(Deleted, deleted.Count);
        }

        [Test(Description = "Tests getting a track by id")]
        public void GetTrackByIdTest()
        {
            var repository = this.TrackRepository();

            var track = this.GenerateTrack();

            var id = repository.Save(track);
            var libraryTrack = repository.GetById(id);

            Assert.AreEqual(id, libraryTrack.Id);
            Assert.AreEqual(track.Path, libraryTrack.Path);
        }

        [Test(Description = "Tests the count of the data stored in the database")]
        public void GetTrackCountTest()
        {
            const int Tracks = 222;
            var repository = this.TrackRepository();
            var tracks = this.GenerateTracks(Tracks);
            repository.Save(tracks);
            var count = repository.GetCount();
            Assert.AreEqual(Tracks, count);
        }

        [Test(Description = "Tests the paginated retrieval of tracks")]
        public void GetTrackPageTest()
        {
            var repository = this.TrackRepository();
            var tracks = this.GenerateTracks(100);
            var saved = repository.Save(tracks);
            Assert.AreEqual(100, saved);
            var page = repository.GetPage(0, 50);
            Assert.AreEqual(50, page.Count);
            Assert.AreEqual(tracks[0].Path, page.First().Path);
            Assert.AreEqual(tracks[49].Path, page.Last().Path);

            var secondPage = repository.GetPage(50, 50);
            Assert.AreEqual(50, secondPage.Count);
            Assert.AreEqual(tracks[50].Path, secondPage.First().Path);
            Assert.AreEqual(tracks[99].Path, secondPage.Last().Path);

            var impossiblePage = repository.GetPage(100, 100);
            Assert.AreEqual(0, impossiblePage.Count);
        }

        [Test(Description = "Test the get album by id function")]
        public void GetTracksByAlbumIdTest()
        {
            var repository = this.TrackRepository();
            var libraryTracks = this.GenerateTracks(50);

            libraryTracks.Take(20).ToList().ForEach(t => t.AlbumId = 10);

            var saveTracks = repository.Save(libraryTracks);
            Assert.AreEqual(saveTracks, libraryTracks.Count);
            var albumTracks = repository.GetTracksByAlbumId(10);
            Assert.GreaterOrEqual(albumTracks.Count, 20);
        }

        [Test(Description = "Test getting the updated entries")]
        public void GetUpdatedTracksTest()
        {
            var repository = this.TrackRepository();

            const int TrackNum = 162;
            const int Updated = 54;
            var tracks = this.GenerateTracks(TrackNum);
            var epoch = DateTime.UtcNow.ToUnixTime();
            tracks.Take(Updated).ToList().ForEach(t => t.DateUpdated = epoch);

            repository.Save(tracks);
            var updatedTracks = repository.GetUpdatedPage(0, TrackNum, epoch);

            Assert.AreEqual(Updated, updatedTracks.Count);
        }

        /// <summary>
        /// The save tracks test.
        /// </summary>
        [Test(Description = "Testing save functionality")]
        public void SaveTracksTest()
        {
            var repository = this.TrackRepository();
            var libraryTracks = this.GenerateTracks(50);
            var tracks = repository.Save(libraryTracks);
            Assert.AreEqual(libraryTracks.Count, tracks);
        }

        [Test(Description = "Tests the save of a single track")]
        public void SaveTrackTest()
        {
            var repository = this.TrackRepository();

            var track = this.GenerateTrack();

            var id = repository.Save(track);
            Assert.AreNotEqual(0, id);
            Assert.AreEqual(1, id);
        }

        [Test(Description = "Test the soft delete function")]
        public void SoftDeleteTracksTest()
        {
            const int TrackNum = 156;
            const int Deleted = 56;
            var repository = this.TrackRepository();
            var tracks = this.GenerateTracks(TrackNum);
            var save = repository.Save(tracks);
            Assert.AreEqual(save, TrackNum);
            var all = repository.GetAll();
            Assert.AreEqual(TrackNum, all.Count);
            var toDelete = all.Take(Deleted).ToList();
            var softDelete = repository.SoftDelete(toDelete);
            Assert.AreEqual(Deleted, softDelete);
            var deleted = repository.GetDeleted();
            Assert.AreEqual(Deleted, deleted.Count);
        }

        [Test()]
        public void TrackRepositoryTest()
        {
            const int TrackNum = 265;
            const int Updated = 13;
            const int Deleted = 35;
            var repository = this.TrackRepository();
            var generated = this.GenerateTracks(TrackNum);
            var saved = repository.Save(generated);

            Assert.AreEqual(TrackNum, saved);

            var epoch = DateTime.UtcNow.ToUnixTime();

            var all = repository.GetAll();
            var toUpdate = all.Take(Updated).ToList();
            toUpdate.ForEach(t => t.Year = "2014");
            var toDelete = all.Skip(Updated).Take(Deleted).ToList();
            var softDeleted = repository.SoftDelete(toDelete);
            Assert.AreEqual(Deleted, softDeleted);
            var update = repository.Save(toUpdate);
            Assert.AreEqual(Updated, update);
            var updated = repository.GetUpdatedPage(0, TrackNum, epoch);
            Assert.AreEqual(Updated, updated.Count);
            var count = updated.Count(t => t.Year == "2014");
            Assert.AreEqual(Updated, count);
        }

        [Test(Description = "Tests the updating of tracks")]
        public void UpdateTracksTest()
        {
            const int TrackNum = 150;
            const int Updated = 21;
            var repository = this.TrackRepository();
            var tracks = this.GenerateTracks(TrackNum);
            var saved = repository.Save(tracks);
            Assert.AreEqual(TrackNum, saved);
            var all = repository.GetAll();
            var toUpdate = all.Take(Updated).ToList();
            toUpdate.ForEach(track => track.Title = "It's ame Mario");
            var updated = repository.Save(toUpdate);
            Assert.AreEqual(Updated, updated);
            var page = repository.GetUpdatedPage(0, TrackNum, 10);
            Assert.AreEqual(Updated, page.Count);
        }

        [Test(Description = "Tests the updating of a track")]
        public void UpdateTrackTest()
        {
            const string Title = "It's ame mario";
            var repository = this.TrackRepository();
            var track = this.GenerateTrack();
            var id = repository.Save(track);
            Assert.AreEqual(1, id);
            var cached = repository.GetById(1);
            Assert.AreEqual(track.Title, cached.Title);
            cached.Title = Title;
            var trackId = repository.Save(cached);
            Assert.AreEqual(1, trackId);
            cached = null;
            Assert.IsNull(cached);
            cached = repository.GetById(1);
            Assert.AreEqual(Title, cached.Title);
            var track2 = this.GenerateTrack();
            var track2Id = repository.Save(track2);
            Assert.AreEqual(2, track2Id);
            var cached2 = repository.GetById(2);
            Assert.AreEqual(track2.Title, cached2.Title);
            cached2.Title = Title;
            var updateId = repository.Save(cached2);
            Assert.AreEqual(2, updateId);
        }
    }
}