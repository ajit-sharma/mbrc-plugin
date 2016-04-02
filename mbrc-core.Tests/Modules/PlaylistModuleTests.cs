using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using Moq;
using MusicBeeRemoteCore.ApiAdapters;
using MusicBeeRemoteData.Entities;
using MusicBeeRemoteData.Repository.Interfaces;
using Ninject;
using Ninject.MockingKernel;
using NUnit.Framework;

namespace MusicBeeRemoteCore.Modules.Tests
{
    [TestFixture]
    public class PlaylistModuleTests
    {
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
            const string path = "/media/music/playlists.mbp";
            const string name = "My super empty playlist";

            var scheduler = new TestScheduler();
        
            Playlist playlist = null;
          
            var kernel = new Ninject.MockingKernel.Moq.MoqMockingKernel();
            kernel.Bind<IPlaylistModule>().To<PlaylistModule>();
            kernel.Bind<IScheduler>().ToMethod(context => scheduler);

            var apiAdapter = kernel.GetMock<IPlaylistApiAdapter>();
            var repository = kernel.GetMock<IPlaylistRepository>();

            apiAdapter.Setup(adapter => adapter.CreatePlaylist(It.IsAny<string>(), It.IsAny<string[]>())).Returns(path);
            repository.Setup(playlistRepository => playlistRepository.Save(It.IsNotNull<Playlist>()))
                .Callback<Playlist>(saved => playlist = saved)
                .Returns(1);

            var module = kernel.Get<IPlaylistModule>();
            scheduler.Start();
            var success = module.CreateNewPlaylist(name, new string[] {});
            scheduler.AdvanceBy(1000);
            
            Assert.True(success);
            Assert.NotNull(playlist);
            Assert.AreEqual(path, playlist.Path);
            Assert.AreEqual(name, playlist.Name);
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
        public void SyncDebugLastPlaylistTest()
        {
            Assert.Fail();
        }

        [Test]
        public void SyncPlaylistsWithCacheTest()
        {
            Assert.Fail();
        }
    }
}