using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using Moq;
using MusicBeeRemoteCore.ApiAdapters;
using MusicBeeRemoteData.Entities;
using MusicBeeRemoteData.Extensions;
using MusicBeeRemoteData.Repository.Interfaces;
using Newtonsoft.Json;
using Ninject;
using Ninject.MockingKernel;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace MusicBeeRemoteCore.Modules.Tests
{
    [TestFixture]
    public class PlaylistModuleTests
    {
        private const string Path = "/media/music/playlists.mbp";
        private const string Name = "My super empty playlist";
        private Fixture fixture;
        private List<PlaylistTrack> _playlistTracks;
        private List<PlaylistTrackInfo> _playlistTrackInfos;

        [SetUp]
        public void Setup()
        {
            fixture = new Fixture();
            var playlistData = File.ReadAllText("Data\\playlist_data.json");
            var trackinfo = File.ReadAllText("Data\\track_info.json");
            _playlistTracks = JsonConvert.DeserializeObject<List<PlaylistTrack>>(playlistData);
            _playlistTrackInfos = JsonConvert.DeserializeObject<List<PlaylistTrackInfo>>(trackinfo);
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void CreateNewPlaylistTest()
        {
            Assert.Fail();
        }

        [Test]
        public void DeleteTrackFromPlaylistTest()
        {
            Assert.Fail();
        }

        [Test]
        public void GetAvailablePlaylistsTest()
        {
            Assert.Fail();
        }

        [Test]
        public void GetPlaylistTracksInfoTest()
        {
            Assert.Fail();
        }

        [Test]
        public void GetPlaylistTracksTest()
        {
            var scheduler = new TestScheduler();
        
            Playlist playlist = null;
          
            var kernel = new Ninject.MockingKernel.Moq.MoqMockingKernel();
            kernel.Bind<IPlaylistModule>().To<PlaylistModule>();
            kernel.Bind<IScheduler>().ToMethod(context => scheduler);

            var apiAdapter = kernel.GetMock<IPlaylistApiAdapter>();
            var repository = kernel.GetMock<IPlaylistRepository>();

            apiAdapter.Setup(adapter => adapter.CreatePlaylist(It.IsAny<string>(), It.IsAny<string[]>())).Returns(Path);
            repository.Setup(playlistRepository => playlistRepository.Save(It.IsNotNull<Playlist>()))
                .Callback<Playlist>(saved => playlist = saved)
                .Returns(1);

            var module = kernel.Get<IPlaylistModule>();
            scheduler.Start();
            var success = module.CreateNewPlaylist(Name, new string[] {});
            scheduler.AdvanceBy(1000);
            
            Assert.True(success);
            Assert.NotNull(playlist);
            Assert.AreEqual(Path, playlist.Path);
            Assert.AreEqual(Name, playlist.Name);
        }

        [Test]
        public void MovePlaylistTrackTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PlaylistAddTracksTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PlaylistDeleteTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PlaylistModuleTest()
        {
            Assert.Fail();
        }

        [Test]
        public void PlaylistPlayNowTest()
        {
            Assert.Fail();
        }
        
        [Test]
        public void SyncPlaylistDataWithCacheTest()
        {
            var scheduler = new TestScheduler();

            var epoch = DateTime.UtcNow.ToUnixTime();
            var playlist = new Playlist
            {
                Id = 1,
                Name = Name,
                Path = Path,
                DateAdded = epoch               
            };

            var kernel = new Ninject.MockingKernel.Moq.MoqMockingKernel();
            kernel.Bind<IPlaylistModule>().To<PlaylistModule>();
            kernel.Bind<IScheduler>().ToMethod(context => scheduler);

            var apiAdapter = kernel.GetMock<IPlaylistApiAdapter>();
            
            var trackRepository = kernel.GetMock<IPlaylistTrackRepository>();
            var trackInfoRepository = kernel.GetMock<IPlaylistTrackInfoRepository>();

            var position = 1;
            var tracks = this.fixture.Build<PlaylistTrackInfo>()
                    .With(t => t.DateAdded, epoch)
                    .With(info => info.Position, position++)
                    .Without(t => t.DateUpdated)
                    .Without(t => t.DateDeleted)
                    .Without(t => t.Id)
                    .CreateMany(15)
                    .ToList();

            var inMemoryRepository = new List<PlaylistTrackInfo>(_playlistTrackInfos);
            var inMemoryTrackRepository = new List<PlaylistTrack>(_playlistTracks);

            var matches = inMemoryRepository.Select(info =>
            {
                var playlistTrack = inMemoryTrackRepository.FirstOrDefault(track => track.TrackInfoId == info.Id);
                info.Position = playlistTrack?.Position ?? 0;
                return info;
            }).ToList();
            
            apiAdapter.Setup(adapter => adapter.GetPlaylistTracks(It.IsAny<string>())).Returns(tracks);
            trackInfoRepository.Setup(repository => repository.GetTracksForPlaylist(It.IsAny<long>())).Returns(matches);
            trackInfoRepository.Setup(repository => repository.GetAll()).Returns(inMemoryRepository);
            trackInfoRepository.Setup(repository => repository.Save(It.IsAny<IList<PlaylistTrackInfo>>()))
                .Callback<IList<PlaylistTrackInfo>>(list => inMemoryRepository.AddRange(list));
            trackRepository.Setup(repository => repository.Delete(It.IsAny<IList<PlaylistTrack>>()))
                .Callback<IList<PlaylistTrack>>(
                    list => list.ToList().ForEach(track => inMemoryTrackRepository.Remove(track)));
            trackRepository.Setup(repository => repository.Save(It.IsAny<IList<PlaylistTrack>>()))
                .Callback<IList<PlaylistTrack>>(track => inMemoryTrackRepository.AddRange(track));
            
        
            var module = kernel.Get<IPlaylistModule>();

            module.SyncPlaylistDataWithCache(playlist);

            Assert.AreEqual(inMemoryRepository.Count, inMemoryTrackRepository.Count);
        }
    }
}