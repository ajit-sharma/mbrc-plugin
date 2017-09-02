using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using Microsoft.Reactive.Testing;
using Moq;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Modules;
using MusicBeeRemote.Data.Entities;
using MusicBeeRemote.Data.Extensions;
using MusicBeeRemote.Data.Repository.Interfaces;
using Newtonsoft.Json;
using NUnit.Framework;
using StructureMap.AutoMocking.Moq;

namespace mbrc_core.Tests.Modules
{
    [TestFixture]
    public class PlaylistModuleTests
    {
        private const string Path = "/media/music/playlists.mbp";
        private const string Name = "My super empty playlist";

        private List<PlaylistTrack> _tracks;
        private List<PlaylistTrackInfo> _playlistTrackInfos;

        private MockRepository _mockRepo;

        [SetUp]
        public void Setup()
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string playlists = "mbrc_core.Tests.Data.playlist_data.json";
            const string trackInfo = "mbrc_core.Tests.Data.track_info.json";

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

            _mockRepo = new MockRepository();
            _mockRepo.Info.AddRange(_playlistTrackInfos);
            _mockRepo.Track.AddRange(_tracks);
        }

        [TearDown]
        public void TearDown()
        {
        }


        [Test]
        public void GetPlaylistTracksTest()
        {
            var scheduler = new TestScheduler();

            Playlist playlist = null;

            var autoModule = new MoqAutoMocker<PlaylistModule>();

            autoModule.Container.Configure(c => { c.For<IScheduler>().Use(() => scheduler); });

            var apiAdapter = Mock.Get(autoModule.Get<IPlaylistApiAdapter>());
            var repository = Mock.Get(autoModule.Get<IPlaylistRepository>());

            apiAdapter.Setup(adapter => adapter.CreatePlaylist(It.IsAny<string>(), It.IsAny<string[]>())).Returns(Path);
            repository.Setup(playlistRepository => playlistRepository.Save(It.IsNotNull<Playlist>()))
                .Callback<Playlist>(saved => playlist = saved)
                .Returns(1);

            var module = autoModule.ClassUnderTest;

            scheduler.Start();
            var success = module.CreateNewPlaylist(Name, new string[] { });
            scheduler.AdvanceBy(1000);

            Assert.True(success);
            Assert.NotNull(playlist);
            Assert.AreEqual(Path, playlist.Url);
            Assert.AreEqual(Name, playlist.Name);
        }

        [Test]
        public void PlaylistPlayNowTest()
        {
            var autoModule = new MoqAutoMocker<PlaylistModule>();
            var apiAdapter = Mock.Get(autoModule.Get<IPlaylistApiAdapter>());
            var module = autoModule.ClassUnderTest;
            apiAdapter.Setup(adapter => adapter.PlayNow(It.IsAny<string>())).Returns(() => true);
            var playNow = module.PlaylistPlayNow(Path);

            Assert.AreEqual(true, playNow);
        }

        [Test]
        public void SyncPlaylistDataWithCacheTestNoUpdateNeeded()
        {
            var epoch = DateTime.UtcNow.ToUnixTime();
            var playlist = new Playlist
            {
                Id = 1,
                Name = Name,
                Url = Path,
                DateAdded = epoch
            };

            var scheduler = new TestScheduler();

            var autoModule = new MoqAutoMocker<PlaylistModule>();
            autoModule.Container.Configure(c => { c.For<IScheduler>().Use(() => scheduler); });

            var apiAdapter = Mock.Get(autoModule.Get<IPlaylistApiAdapter>());
            var trackRepository = Mock.Get(autoModule.Get<IPlaylistTrackRepository>());
            var trackInfoRepository = Mock.Get(autoModule.Get<IPlaylistTrackInfoRepository>());
            var playlistRepository = Mock.Get(autoModule.Get<IPlaylistRepository>());

            var matches = _mockRepo.GetPlaylistTracksJoin(true);

            apiAdapter.SetupSequence(a => a.GetPlaylistTracks(It.IsAny<string>()))
                .Returns(matches.ToList())
                .Returns(matches);
            
            trackRepository.Setup(r => r.GetTracksForPlaylist(It.IsAny<int>()))
                .Returns(_tracks.ToList());
            
            trackInfoRepository.Setup(r => r.GetTracksForPlaylist(It.IsAny<int>()))
                .Returns(_mockRepo.GetPlaylistTracksJoin(false));
            
            trackInfoRepository.Setup(r => r.GetAll()).Returns(_mockRepo.Info);
            
            trackInfoRepository.Setup(r => r.Save(It.IsAny<IList<PlaylistTrackInfo>>()))
                .Callback<IList<PlaylistTrackInfo>>(list => _mockRepo.Info.AddRange(list));
            
            trackRepository.Setup(r => r.Delete(It.IsAny<IList<PlaylistTrack>>()))
                .Callback<IList<PlaylistTrack>>(list => list.ToList().ForEach(track => _mockRepo.Track.Remove(track)));
            
            trackRepository.Setup(r => r.Save(It.IsAny<IList<PlaylistTrack>>()))
                .Callback<IList<PlaylistTrack>>(track => _mockRepo.Track.AddRange(track));

            playlistRepository.Setup(r => r.Save(It.IsAny<Playlist>()))
                .Callback<Playlist>(playlist1 => playlist = playlist1)
                .Returns(1);

            var module = autoModule.ClassUnderTest;

            var equal = module.SyncPlaylistDataWithCache(playlist);

            Assert.AreEqual(0, playlist.DateUpdated);
            Assert.AreEqual(_playlistTrackInfos.Count, _mockRepo.Info.Count);
            Assert.AreEqual(_tracks.Count, _mockRepo.Track.Count);
            Assert.IsTrue(equal);
        }

        [Test]
        public void SyncPlaylistDataWithCacheTestUpdateNeededDuplicates()
        {
            var epoch = DateTime.UtcNow.ToUnixTime();
            var playlist = new Playlist
            {
                Id = 1,
                Name = Name,
                Url = Path,
                DateAdded = epoch
            };

            var scheduler = new TestScheduler();
            var autoModule = new MoqAutoMocker<PlaylistModule>();
            autoModule.Container.Configure(c => { c.For<IScheduler>().Use(() => scheduler); });


            var apiAdapter = Mock.Get(autoModule.Get<IPlaylistApiAdapter>());
            var trackRepository = Mock.Get(autoModule.Get<IPlaylistTrackRepository>());
            var trackInfoRepository = Mock.Get(autoModule.Get<IPlaylistTrackInfoRepository>());
            var playlistRepository = Mock.Get(autoModule.Get<IPlaylistRepository>());

            var matches = _mockRepo.GetPlaylistTracksJoin(true);


            var position = matches.Select(lt => lt.Position).OrderBy(i => i).LastOrDefault();
            var item = matches[5];

            for (var i = 0; i < 4; i++)
            {
                var info = new PlaylistTrackInfo
                {
                    Id = 0,
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

            trackRepository.Setup(repository => repository.GetTracksForPlaylist(It.IsAny<int>()))
                .Returns(_mockRepo.Track);

            trackInfoRepository.Setup(repository => repository.GetTracksForPlaylist(It.IsAny<int>()))
                .Returns(() => _mockRepo.GetPlaylistTracksJoin(false));

            trackInfoRepository.Setup(repository => repository.GetAll()).Returns(_mockRepo.Info);
            trackInfoRepository.Setup(repository => repository.Save(It.IsAny<IList<PlaylistTrackInfo>>()))
                .Callback<IList<PlaylistTrackInfo>>(list => MockRepository.Save(list, _mockRepo.Info));
            trackRepository.Setup(repository => repository.Delete(It.IsAny<IList<PlaylistTrack>>()))
                .Callback<IList<PlaylistTrack>>(
                    list => list.ToList().ForEach(track => _mockRepo.Track.Remove(track)));
            trackRepository.Setup(repository => repository.Save(It.IsAny<IList<PlaylistTrack>>()))
                .Callback<IList<PlaylistTrack>>(track => MockRepository.Save(track, _mockRepo.Track));

            playlistRepository.Setup(repository => repository.Save(It.IsAny<Playlist>()))
                .Callback<Playlist>(playlist1 =>
                {
                    playlist = playlist1;
                    playlist.DateUpdated = DateTime.UtcNow.ToUnixTime();
                })
                .Returns(1);

            var module = autoModule.ClassUnderTest;

            var equal = module.SyncPlaylistDataWithCache(playlist);

            Assert.Greater(playlist.DateUpdated, 0);
            Assert.AreEqual(_playlistTrackInfos.Count, _mockRepo.Info.Count);
            Assert.AreEqual(matches.Count, _mockRepo.Track.Count);
            Assert.IsTrue(equal);
        }
    }


    internal class MockRepository
    {
        public List<PlaylistTrackInfo> Info { get; } = new List<PlaylistTrackInfo>();
        public List<PlaylistTrack> Track { get; } = new List<PlaylistTrack>();

        public static void Save<T>(IList<T> items, IList<T> repo) where T : TypeBase
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

        public List<PlaylistTrackInfo> GetPlaylistTracksJoin(bool fromApi)
        {
            var data = Track.Select(track =>
            {
                var first = Info.FirstOrDefault(info => info.Id == track.TrackInfoId);
                if (first != null)
                {
                    return new PlaylistTrackInfo
                    {
                        Position = track.Position,
                        Artist = first.Artist,
                        Title = first.Title,
                        Path = first.Path,
                        Id = fromApi ? 0 : track.Id
                    };
                }
                return null;
            }).ToList();
            data.Sort((info, trackInfo) => info.Position.CompareTo(trackInfo.Position));
            return data;
        }
    }
}