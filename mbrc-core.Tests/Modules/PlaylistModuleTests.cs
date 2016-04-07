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
                _tracks = JsonConvert.DeserializeObject<List<PlaylistTrack>>(reader.ReadToEnd());
            }

            using (var stream = assembly.GetManifestResourceStream(trackInfo))
            using (var reader = new StreamReader(stream))
            {
                _playlistTrackInfos = JsonConvert.DeserializeObject<List<PlaylistTrackInfo>>(reader.ReadToEnd());
            }

            mockRepo = new MockRepository();
            mockRepo.Info.AddRange(_playlistTrackInfos);
            mockRepo.Track.AddRange(_tracks);
        }

        [TearDown]
        public void TearDown()
        {
        }

        private const string Path = "/media/music/playlists.mbp";
        private const string Name = "My super empty playlist";
        private Fixture fixture;
        private List<PlaylistTrack> _tracks;
        private List<PlaylistTrackInfo> _playlistTrackInfos;
        private MoqMockingKernel kernel;
        private MockRepository mockRepo;

        private List<PlaylistTrackInfo> GetPlaylistTracksJoin(bool fromApi)
        {
            var data = mockRepo.Track.Select(track =>
            {
                var first = mockRepo.Info.FirstOrDefault(info => info.Id == track.TrackInfoId);
                if (first != null)
                {
                    return new PlaylistTrackInfo
                    {
                        Position = track.Position,
                        Artist = first.Artist,
                        Title = first.Title,
                        Path = first.Path,
                        Id =  fromApi ? 0 : track.Id
                    };
                }
                return null;
            }).ToList();
            data.Sort((info, trackInfo) => info.Position.CompareTo(trackInfo.Position));
            return data;
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
        public void SyncPlaylistDataWithCacheTestNoUpdateNeeded()
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

            var matches = GetPlaylistTracksJoin(true);
            
            apiAdapter.SetupSequence(adapter => adapter.GetPlaylistTracks(It.IsAny<string>()))
                .Returns(matches.ToList())
                .Returns(matches);
            trackRepository.Setup(repository => repository.GetTracksForPlaylist(It.IsAny<long>()))
                .Returns(_tracks.ToList());
            trackInfoRepository.Setup(repository => repository.GetTracksForPlaylist(It.IsAny<long>())).Returns(GetPlaylistTracksJoin(false));
            trackInfoRepository.Setup(repository => repository.GetAll()).Returns(mockRepo.Info);
            trackInfoRepository.Setup(repository => repository.Save(It.IsAny<IList<PlaylistTrackInfo>>()))
                .Callback<IList<PlaylistTrackInfo>>(list => mockRepo.Info.AddRange(list));
            trackRepository.Setup(repository => repository.Delete(It.IsAny<IList<PlaylistTrack>>()))
                .Callback<IList<PlaylistTrack>>(
                    list => list.ToList().ForEach(track => mockRepo.Track.Remove(track)));
            trackRepository.Setup(repository => repository.Save(It.IsAny<IList<PlaylistTrack>>()))
                .Callback<IList<PlaylistTrack>>(track => mockRepo.Track.AddRange(track));

            playlistRepository.Setup(repository => repository.Save(It.IsAny<Playlist>()))
                .Callback<Playlist>(playlist1 => playlist = playlist1)
                .Returns(1);

            var module = kernel.Get<IPlaylistModule>();

            var equal = module.SyncPlaylistDataWithCache(playlist);

            Assert.AreEqual(0, playlist.DateUpdated);
            Assert.AreEqual(_playlistTrackInfos.Count, mockRepo.Info.Count);
            Assert.AreEqual(_tracks.Count, mockRepo.Track.Count);
            Assert.IsTrue(equal);
        }

        [Test]
        public void SyncPlaylistDataWithCacheTestUpdateNeededDuplicates()
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

            var matches = GetPlaylistTracksJoin(true);
            

            var position = matches.Select(lt => lt.Position).OrderBy(i => i).LastOrDefault();
            var item = matches[5];

            for (var i = 0; i < 4; i++)
            {
                var info = new PlaylistTrackInfo
                {
                    Id =  0,
                    Artist = item.Artist,
                    Title = item.Title,
                    Path = item.Path,
                    Position = ++position
                };
                matches.Add(info);
            }


            matches.Sort((info, trackInfo) => info.Position.CompareTo(trackInfo.Position));

            apiAdapter.SetupSequence(adapter => adapter.GetPlaylistTracks(It.IsAny<string>()))
                .Returns(matches.ToList())
                .Returns(matches);
            trackRepository.Setup(repository => repository.GetTracksForPlaylist(It.IsAny<long>()))
                .Returns(_tracks.ToList());
            trackInfoRepository.Setup(repository => repository.GetTracksForPlaylist(It.IsAny<long>())).Returns(() => GetPlaylistTracksJoin(false));
            trackInfoRepository.Setup(repository => repository.GetAll()).Returns(mockRepo.Info);
            trackInfoRepository.Setup(repository => repository.Save(It.IsAny<IList<PlaylistTrackInfo>>()))
                .Callback<IList<PlaylistTrackInfo>>(list => MockRepository.Save(list, mockRepo.Info));
            trackRepository.Setup(repository => repository.Delete(It.IsAny<IList<PlaylistTrack>>()))
                .Callback<IList<PlaylistTrack>>(
                    list => list.ToList().ForEach(track => mockRepo.Track.Remove(track)));
            trackRepository.Setup(repository => repository.Save(It.IsAny<IList<PlaylistTrack>>()))
                .Callback<IList<PlaylistTrack>>(track => MockRepository.Save(track, mockRepo.Track));

            playlistRepository.Setup(repository => repository.Save(It.IsAny<Playlist>()))
                .Callback<Playlist>(playlist1 =>
                {
                    playlist = playlist1;
                    playlist.DateUpdated = DateTime.UtcNow.ToUnixTime();
                })
                .Returns(1);

            var module = kernel.Get<IPlaylistModule>();

            var equal = module.SyncPlaylistDataWithCache(playlist);

            Assert.Greater(playlist.DateUpdated, 0);
            Assert.AreEqual(_playlistTrackInfos.Count, mockRepo.Info.Count);
            Assert.AreEqual(matches.Count, mockRepo.Track.Count);
            Assert.IsTrue(equal);
        }
    }





    internal class MockRepository
    {
        public List<PlaylistTrackInfo> Info { get; } = new List<PlaylistTrackInfo>();
        public List<PlaylistTrack> Track { get; } = new List<PlaylistTrack>();

        public static void Save<T>(IList<T> items, IList<T> repo) where T: TypeBase
        {
            items.ToList().ForEach(item =>
            {
                if (item.Id > 0)
                {
                    var match = repo.FirstOrDefault(stored => stored.Id == item.Id);
                    if (match != null)
                    {
                        var indexOf = repo.IndexOf(match);
                        repo.Remove(match);
                        repo.Insert(indexOf, item);
                        return;
                    }
                }

                var id = repo.Select(lt => lt.Id).OrderBy(l => l).LastOrDefault();
                item.Id = ++id;
                repo.Add(item);
            });
        }
    }
    
}