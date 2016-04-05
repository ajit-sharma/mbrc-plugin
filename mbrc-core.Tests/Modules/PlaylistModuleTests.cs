using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using Microsoft.Reactive.Testing;
using Moq;
using MusicBeeRemoteCore.ApiAdapters;
using MusicBeeRemoteData.Entities;
using MusicBeeRemoteData.Extensions;
using MusicBeeRemoteData.Repository.Interfaces;
using Newtonsoft.Json;
using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace MusicBeeRemoteCore.Modules.Tests
{
    [TestFixture]
    public class PlaylistModuleTests
    {
        [SetUp]
        public void Setup()
        {
            kernel = new MoqMockingKernel();
            fixture = new Fixture();
            var assembly = Assembly.GetExecutingAssembly();
            var playlists = "mbrc_core.Tests.Data.playlist_data.json";
            var trackInfo = "mbrc_core.Tests.Data.track_info.json";
                    
            using (var stream = assembly.GetManifestResourceStream(playlists))
            using (var reader = new StreamReader(stream))
            {
                _playlistTracks = JsonConvert.DeserializeObject<List<PlaylistTrack>>(reader.ReadToEnd());
            }

            using (var stream = assembly.GetManifestResourceStream(trackInfo))
            using (var reader = new StreamReader(stream))
            {
                _playlistTrackInfos = JsonConvert.DeserializeObject<List<PlaylistTrackInfo>>(reader.ReadToEnd());
            }
        }

        [TearDown]
        public void TearDown()
        {
        }

        private const string Path = "/media/music/playlists.mbp";
        private const string Name = "My super empty playlist";
        private Fixture fixture;
        private List<PlaylistTrack> _playlistTracks;
        private List<PlaylistTrackInfo> _playlistTrackInfos;
        private MoqMockingKernel kernel;

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

            var kernel = new MoqMockingKernel();
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
            kernel.Reset();
            var scheduler = new TestScheduler();

            var epoch = DateTime.UtcNow.ToUnixTime();
            var playlist = new Playlist
            {
                Id = 1,
                Name = Name,
                Path = Path,
                DateAdded = epoch
            };


            kernel.Bind<IPlaylistModule>().To<PlaylistModule>();
            kernel.Bind<IScheduler>().ToMethod(context => scheduler);

            var apiAdapter = kernel.GetMock<IPlaylistApiAdapter>();        
            var trackRepository = kernel.GetMock<IPlaylistTrackRepository>();
            var trackInfoRepository = kernel.GetMock<IPlaylistTrackInfoRepository>();
            var playlistRepository = kernel.GetMock<IPlaylistRepository>();
            
            var inMemoryRepository = new List<PlaylistTrackInfo>(_playlistTrackInfos.ToList());
            var inMemoryTrackRepository = new List<PlaylistTrack>(_playlistTracks.ToList());

            var matches = _playlistTracks.Select(track =>
            {
                var first = inMemoryRepository.FirstOrDefault(info => info.Id == track.TrackInfoId);
                if (first != null)
                {
                    first.Position = track.Position;
                }                
                return first;
            }).ToList();
            
            apiAdapter.Setup(adapter => adapter.GetPlaylistTracks(It.IsAny<string>())).Returns(matches.ToList());
            trackRepository.Setup(repository => repository.GetTracksForPlaylist(It.IsAny<long>())).Returns(_playlistTracks.ToList());
            trackInfoRepository.Setup(repository => repository.GetTracksForPlaylist(It.IsAny<long>())).Returns(matches.ToList());
            trackInfoRepository.Setup(repository => repository.GetAll()).Returns(inMemoryRepository);
            trackInfoRepository.Setup(repository => repository.Save(It.IsAny<IList<PlaylistTrackInfo>>()))
                .Callback<IList<PlaylistTrackInfo>>(list => inMemoryRepository.AddRange(list));
            trackRepository.Setup(repository => repository.Delete(It.IsAny<IList<PlaylistTrack>>()))
                .Callback<IList<PlaylistTrack>>(
                    list => list.ToList().ForEach(track => inMemoryTrackRepository.Remove(track)));
            trackRepository.Setup(repository => repository.Save(It.IsAny<IList<PlaylistTrack>>()))
                .Callback<IList<PlaylistTrack>>(track => inMemoryTrackRepository.AddRange(track));

            playlistRepository.Setup(repository => repository.Save(It.IsAny<Playlist>())).Callback<Playlist>(playlist1 => playlist = playlist1).Returns(1);


            var module = kernel.Get<IPlaylistModule>();

            var equal = module.SyncPlaylistDataWithCache(playlist);
           
            Assert.AreEqual(0, playlist.DateUpdated);
            Assert.AreEqual(_playlistTrackInfos.Count, inMemoryRepository.Count);
            Assert.AreEqual(_playlistTracks.Count, inMemoryTrackRepository.Count);
            Assert.IsTrue(equal);
        }
    }
}