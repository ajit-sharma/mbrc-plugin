using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MusicBeeRemote.Data;
using MusicBeeRemote.Data.Entities;
using MusicBeeRemote.Data.Extensions;
using MusicBeeRemote.Data.Repository;
using MusicBeeRemote.Data.Repository.Interfaces;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace mbrc_data.Tests.Repository
{
    [TestFixture(Description = "Test the track repository functionality")]
    public class TrackRepositoryTests
    {
        private string _directory;

        private Fixture _fixture;

        private DatabaseManager _databaseManager;
        
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
            _databaseManager.DeleteDatabase();
        }      

        /// <summary>
        /// Creates and returns a new <see cref="ITrackRepository"/> with a clean
        /// database.
        /// </summary>
        /// <returns>
        /// The <see cref="ITrackRepository"/>.
        /// </returns>
        private ITrackRepository TrackRepository()
        {
            _databaseManager = new DatabaseManager(_directory);
            _databaseManager.DeleteDatabase();
            ITrackRepository repository = new TrackRepository(_databaseManager);
            return repository;
        }

        private List<TrackDao> GenerateTracks(int count)
        {
            var epoch = DateTime.UtcNow.ToUnixTime() - 60;
            return
                _fixture.Build<TrackDao>()
                    .With(t => t.DateAdded, epoch)
                    .Without(t => t.DateUpdated)
                    .Without(t => t.DateDeleted)
                    .Without(t => t.Id)
                    .CreateMany(count)
                    .ToList();
        }

        private TrackDao GenerateTrack()
        {
            var epoch = DateTime.UtcNow.ToUnixTime() - 60;
            return
                _fixture.Build<TrackDao>()
                    .With(t => t.DateAdded, epoch)
                    .Without(t => t.DateUpdated)
                    .Without(t => t.DateDeleted)
                    .Without(t => t.Id)
                    .Create();
        }

        [Test(Description = "Some of the ids included should be invalid")]
        public void DeleteTracksInvalid()
        {
            var repository = TrackRepository();
            var libraryTracks = GenerateTracks(100);
            var affectedRows = repository.Save(libraryTracks);
            Assert.AreEqual(100, affectedRows);

            var tracks = repository.GetAll();
            Assert.AreEqual(100, tracks.Count);

            tracks.Add(new TrackDao { Id = 19321 });
            tracks.Add(new TrackDao { Id = 1249 });

            tracks = tracks.Reverse().ToList();

            var deletedRows = repository.Delete(tracks);
            Assert.AreEqual(100, deletedRows);
            var allTracks = repository.GetAll();
            Assert.AreEqual(0, allTracks.Count);
        }

        [Test(Description = "Delete all valid id tracks")]
        public void DeleteTracksTest()
        {
            var repository = TrackRepository();
            var libraryTracks = GenerateTracks(100);
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
            const int totalTracks = 156;
            const int deletedTracks = 50;
            var repository = TrackRepository();
            var tracks = GenerateTracks(totalTracks);
            var epoch = DateTime.UtcNow.ToUnixTime();
            tracks.Take(deletedTracks).ToList().ForEach(track => { track.DateDeleted = epoch; });
            repository.Save(tracks);
            var all = repository.GetAll();
            Assert.AreEqual(totalTracks, all.Count);
            var deleted = repository.GetDeleted();
            Assert.AreEqual(deletedTracks, deleted.Count);
        }

        [Test(Description = "Test the retrieval of cached")]
        public void GetCachedTracksTest()
        {
            const int totalTracks = 156;
            const int deletedTracks = 50;
            var repository = TrackRepository();
            var tracks = GenerateTracks(totalTracks);
            var epoch = DateTime.UtcNow.ToUnixTime();
            tracks.Take(deletedTracks).ToList().ForEach(track => { track.DateDeleted = epoch; });
            repository.Save(tracks);

            var cached = repository.GetCached();
            Assert.AreEqual(totalTracks - deletedTracks, cached.Count);
        }

        [Test]
        public void GetDeletedTracksTest()
        {
            const int totalTracks = 156;
            const int deletedTracks = 50;
            var repository = TrackRepository();
            var tracks = GenerateTracks(totalTracks);
            var epoch = DateTime.UtcNow.ToUnixTime();
            tracks.Take(deletedTracks).ToList().ForEach(track => { track.DateDeleted = epoch; });
            repository.Save(tracks);

            var deleted = repository.GetDeleted();
            Assert.AreEqual(deletedTracks, deleted.Count);
        }

        [Test(Description = "Tests getting a track by id")]
        public void GetTrackByIdTest()
        {
            var repository = TrackRepository();

            var track = GenerateTrack();

            var id = repository.Save(track);
            var libraryTrack = repository.GetById(id);

            Assert.AreEqual(id, libraryTrack.Id);
            Assert.AreEqual(track.Path, libraryTrack.Path);
        }

        [Test(Description = "Tests the count of the data stored in the database")]
        public void GetTrackCountTest()
        {
            const int totalTracks = 222;
            var repository = TrackRepository();
            var startCount = repository.GetCount();
            Assert.AreEqual(0, startCount);
            var tracks = GenerateTracks(totalTracks);
            repository.Save(tracks);
            var count = repository.GetCount();
            Assert.AreEqual(totalTracks, count);
        }

        [Test(Description = "Tests the paginated retrieval of tracks")]
        public void GetTrackPageTest()
        {
            var repository = TrackRepository();
            var tracks = GenerateTracks(100);
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
            var repository = TrackRepository();
            var libraryTracks = GenerateTracks(50);

            libraryTracks.Take(20).ToList().ForEach(t => t.AlbumId = 10);

            var saveTracks = repository.Save(libraryTracks);
            Assert.AreEqual(saveTracks, libraryTracks.Count);
            var albumTracks = repository.GetTracksByAlbumId(10);
            Assert.GreaterOrEqual(albumTracks.Count, 20);
        }

        [Test(Description = "Test getting the updated entries")]
        public void GetUpdatedTracksTest()
        {
            var repository = TrackRepository();

            const int totalTracks = 162;
            const int tracksUpdated = 54;
            var tracks = GenerateTracks(totalTracks);
            var saved = repository.Save(tracks);
            Assert.AreEqual(totalTracks, saved);
            var epoch = DateTime.UtcNow.ToUnixTime();
            var storedTracks = repository.GetAll();
            var update = storedTracks.Take(tracksUpdated).ToList();
            update.ForEach(t => t.AlbumArtistId = 5);

            var updated = repository.Save(update);
            Assert.AreEqual(tracksUpdated, updated);
            var updatedTracks = repository.GetUpdatedPage(0, totalTracks, epoch);
            Assert.AreEqual(tracksUpdated, updatedTracks.Count);
            Assert.That(updatedTracks.Count(x => x.AlbumArtistId == 5), Is.EqualTo(tracksUpdated));
        }

        /// <summary>
        /// The save tracks test.
        /// </summary>
        [Test(Description = "Testing save functionality")]
        public void SaveTracksTest()
        {
            var repository = TrackRepository();
            var libraryTracks = GenerateTracks(50);
            var tracks = repository.Save(libraryTracks);
            Assert.AreEqual(libraryTracks.Count, tracks);
        }

        [Test(Description = "Tests the save of a single track")]
        public void SaveTrackTest()
        {
            var repository = TrackRepository();

            var track = GenerateTrack();

            var id = repository.Save(track);
            Assert.AreNotEqual(0, id);
            Assert.AreEqual(1, id);
        }

        [Test(Description = "Test the soft delete function")]
        public void SoftDeleteTracksTest()
        {
            const int totalTracks = 156;
            const int tracksDeleted = 56;
            var repository = TrackRepository();
            var tracks = GenerateTracks(totalTracks);
            var save = repository.Save(tracks);
            Assert.AreEqual(save, totalTracks);
            var all = repository.GetAll();
            Assert.AreEqual(totalTracks, all.Count);
            var toDelete = all.Take(tracksDeleted).ToList();
            var softDelete = repository.SoftDelete(toDelete);
            Assert.AreEqual(tracksDeleted, softDelete);
            var deleted = repository.GetDeleted();
            Assert.AreEqual(tracksDeleted, deleted.Count);
        }

        [Test]
        public void TrackRepositoryTest()
        {
            const int totalTracks = 265;
            const int updatedTracks = 13;
            const int deletedTracks = 35;
            var repository = TrackRepository();
            var generated = GenerateTracks(totalTracks);
            var saved = repository.Save(generated);
        
            Assert.AreEqual(totalTracks, saved);

            var epoch = DateTime.UtcNow.ToUnixTime();

            var all = repository.GetAll();
            var toUpdate = all.Take(updatedTracks).ToList();
            toUpdate.ForEach(t => t.Year = "2014");
            var toDelete = all.Skip(updatedTracks).Take(deletedTracks).ToList();
            var softDeleted = repository.SoftDelete(toDelete);
            Assert.AreEqual(deletedTracks, softDeleted);
            var update = repository.Save(toUpdate);
            Assert.AreEqual(updatedTracks, update);
            var updated = repository.GetUpdatedPage(0, totalTracks, epoch);
            Assert.AreEqual(updatedTracks + deletedTracks, updated.Count);
            var count = updated.Count(t => t.Year == "2014");
            Assert.AreEqual(updatedTracks, count);
        }

        [Test(Description = "Tests the updating of tracks")]
        public void UpdateTracksTest()
        {
            const int totalTracks = 150;
            const int updatedTracks = 21;
            var repository = TrackRepository();
            var tracks = GenerateTracks(totalTracks);
            var saved = repository.Save(tracks);
            Assert.AreEqual(totalTracks, saved);
            var all = repository.GetAll();
            var toUpdate = all.Take(updatedTracks).ToList();
            toUpdate.ForEach(track => track.Title = "It's ame Mario");
            var epoch = DateTime.UtcNow.ToUnixTime();
            var updated = repository.Save(toUpdate);
            Assert.AreEqual(updatedTracks, updated);
            var page = repository.GetUpdatedPage(0, totalTracks, epoch);
            Assert.AreEqual(updatedTracks, page.Count);
        }

        [Test(Description = "Tests the updating of a track")]
        public void UpdateTrackTest()
        {
            const string title = "It's ame mario";
            var repository = TrackRepository();
            var track = GenerateTrack();
            var id = repository.Save(track);
            Assert.AreEqual(1, id);
            var cached = repository.GetById(1);
            Assert.AreEqual(track.Title, cached.Title);
            cached.Title = title;
            var trackId = repository.Save(cached);
            Assert.AreEqual(1, trackId);
            cached = null;
            Assert.IsNull(cached);
            cached = repository.GetById(1);
            Assert.AreEqual(title, cached.Title);
            var track2 = GenerateTrack();
            var track2Id = repository.Save(track2);
            Assert.AreEqual(2, track2Id);
            var cached2 = repository.GetById(2);
            Assert.AreEqual(track2.Title, cached2.Title);
            cached2.Title = title;
            var updateId = repository.Save(cached2);
            Assert.AreEqual(2, updateId);
        }
    }
}