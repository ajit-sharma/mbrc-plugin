namespace MusicBeePlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MusicBeePlugin.AndroidRemote.Enumerations;
    using MusicBeePlugin.Model;
    using MusicBeePlugin.Rest.ServiceInterface;
    using MusicBeePlugin.Rest.ServiceModel.Enum;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    class PlayerApiAdapter : IPlayerApiAdapter
    {
        public Plugin.MusicBeeApiInterface api;

        /// <summary>
        ///     The fields (MetaData) that MusicBee Remote caches.
        /// </summary>
        private readonly Plugin.MetaDataType[] fields =
            {
                Plugin.MetaDataType.Artist, Plugin.MetaDataType.AlbumArtist, 
                Plugin.MetaDataType.Album, Plugin.MetaDataType.Genre, 
                Plugin.MetaDataType.TrackTitle, Plugin.MetaDataType.Year, 
                Plugin.MetaDataType.TrackNo, Plugin.MetaDataType.DiscNo
            };

        public PlayerApiAdapter(Plugin.MusicBeeApiInterface api)
        {
            this.api = api;
        }

        public bool ChangeRepeat()
        {
            var repeat = this.api.Player_GetRepeat();
            Plugin.RepeatMode newMode;
            switch (repeat)
            {
                case Plugin.RepeatMode.None:
                    newMode = Plugin.RepeatMode.All;
                    break;
                case Plugin.RepeatMode.All:
                    newMode = Plugin.RepeatMode.One;
                    break;
                default:
                    newMode = Plugin.RepeatMode.None;
                    break;
            }

            return this.api.Player_SetRepeat(newMode);
        }

        public ICollection<LibraryAlbum> GetAlbumList()
        {
            var list = new List<LibraryAlbum>();
            if (this.api.Library_QueryLookupTable("album", "count", null))
            {
                list.AddRange(
                    this.api.Library_QueryGetLookupTableValue(null)
                        .Split(new[] { "\0\0" }, StringSplitOptions.None)
                        .Select(artist => new LibraryAlbum { Name = artist.Split('\0')[0] }));
            }

            this.api.Library_QueryLookupTable(null, null, null);
            return list;
        }

        public ICollection<LibraryArtist> GetArtistList()
        {
            var artists = new List<LibraryArtist>();
            if (this.api.Library_QueryLookupTable("artist", "count", string.Empty))
            {
                artists.AddRange(
                    this.api.Library_QueryGetLookupTableValue(null)
                        .Split(new[] { "\0\0" }, StringSplitOptions.None)
                        .Select(entry => entry.Split('\0'))
                        .Select(artistInfo => new LibraryArtist(artistInfo[0])));
            }

            this.api.Library_QueryLookupTable(null, null, null);

            return artists;
        }

        public string GetArtistUrl(string name)
        {
            string[] urls = { };
            this.api.Library_GetArtistPictureUrls(name, true, ref urls);
            return urls.Length > 0 ? urls[0] : string.Empty;
        }

        public byte[] GetCoverData(string path)
        {
            string coverUrl = null;
            var locations = Plugin.PictureLocations.None;
            byte[] imageData = { };
            this.api.Library_GetArtworkEx(path, 0, true, ref locations, ref coverUrl, ref imageData);
            return imageData;
        }

        public string GetCoverUrl(string path)
        {
            string coverUrl = null;
            var locations = Plugin.PictureLocations.None;
            byte[] imageData = { };
            this.api.Library_GetArtworkEx(path, 0, false, ref locations, ref coverUrl, ref imageData);
            return coverUrl;
        }

        public ICollection<LibraryGenre> GetGenreList()
        {
            var list = new List<LibraryGenre>();
            if (this.api.Library_QueryLookupTable("genre", "count", null))
            {
                list.AddRange(
                    this.api.Library_QueryGetLookupTableValue(null)
                        .Split(new[] { "\0\0" }, StringSplitOptions.None)
                        .Select(artist => new LibraryGenre(artist.Split('\0')[0])));
            }

            this.api.Library_QueryLookupTable(null, null, null);
            return list;
        }

        public string[] GetLibraryFiles()
        {
            string[] files = { };
            this.api.Library_QueryFilesEx(string.Empty, ref files);
            return files;
        }

        public ICollection<NowPlaying> GetNowPlayingList()
        {
            this.api.NowPlayingList_QueryFiles(null);

            var tracks = new List<NowPlaying>();
            var position = 1;

            while (true)
            {
                var playListTrack = this.api.NowPlayingList_QueryGetNextFile();
                if (string.IsNullOrEmpty(playListTrack))
                {
                    break;
                }

                var artist = this.api.Library_GetFileTag(playListTrack, Plugin.MetaDataType.Artist);
                var title = this.api.Library_GetFileTag(playListTrack, Plugin.MetaDataType.TrackTitle);

                if (string.IsNullOrEmpty(artist))
                {
                    artist = "Unknown Artist";
                }

                if (string.IsNullOrEmpty(title))
                {
                    var index = playListTrack.LastIndexOf('\\');
                    title = playListTrack.Substring(index + 1);
                }

                var nowPlaying = new NowPlaying
                                     {
                                         Artist = artist, 
                                         Id = position, 
                                         Path = playListTrack, 
                                         Position = position, 
                                         Title = title
                                     };

                tracks.Add(nowPlaying);
                position++;
            }

            return tracks;
        }

        public OutputDevice GetOutputDevices()
        {
            string[] devices;
            string active;
            this.api.Player_GetOutputDevices(out devices, out active);

            return new OutputDevice { Active = active, Devices = devices };
        }

        public PlayerStatus GetStatus()
        {
            return new PlayerStatus
                       {
                           Repeat = this.api.Player_GetRepeat().ToString().ToLower(), 
                           Mute = this.api.Player_GetMute(), 
                           Shuffle = GetShuffleState(), 
                           Scrobble = this.api.Player_GetScrobbleEnabled(), 
                           PlayerState = this.api.Player_GetPlayState().ToString().ToLower(), 
                           Volume = (int)Math.Round(this.api.Player_GetVolume() * 100, 1), 
                           Code = ApiCodes.Success
                       };
        }

        public Modifications GetSyncDelta(string[] cachedFiles, DateTime lastSync)
        {
            string[] newFiles = { };
            string[] deletedFiles = { };
            string[] updatedFiles = { };

            this.api.Library_GetSyncDelta(
                cachedFiles, 
                lastSync, 
                Plugin.LibraryCategory.Music, 
                ref newFiles, 
                ref updatedFiles, 
                ref deletedFiles);
            return new Modifications(deletedFiles, newFiles, updatedFiles);
        }

        public LibraryTrackEx GetTags(string file)
        {
            string[] tags = { };
            this.api.Library_GetFileTags(file, this.fields, ref tags);
            return new LibraryTrackEx(tags);
        }

        public int GetVolume()
        {
            return (int)Math.Round(this.api.Player_GetVolume() * 100, 1);
        }

        public bool NowPlayingMoveTrack(int @from, int to)
        {
            int[] aFrom = { @from };
            int dIn;
            if (@from > to)
            {
                dIn = to - 1;
            }
            else
            {
                dIn = to;
            }

            return this.api.NowPlayingList_MoveFiles(aFrom, dIn);
        }

        public bool NowPlayingRemove(int index)
        {
            return this.api.NowPlayingList_RemoveAt(index);
        }

        public bool PlayNext()
        {
            return this.api.Player_PlayNextTrack();
        }

        public bool PlayNow(string path)
        {
            return this.api.NowPlayingList_PlayNow(path);
        }

        public bool PlayPause()
        {
            return this.api.Player_PlayPause();
        }

        public bool PlayPrevious()
        {
            return this.api.Player_PlayPreviousTrack();
        }

        public bool QueueLast(string[] tracklist)
        {
            return this.api.NowPlayingList_QueueFilesLast(tracklist);
        }

        public bool QueueNext(string[] tracklist)
        {
            return this.api.NowPlayingList_QueueFilesNext(tracklist);
        }

        public bool QueueNow(string[] tracklist)
        {
            this.api.NowPlayingList_Clear();
            var success = this.api.NowPlayingList_QueueFilesNext(tracklist);

            if (tracklist != null && tracklist.Length > 0)
            {
                this.api.NowPlayingList_PlayNow(tracklist[0]);
            }

            return success;
        }

        public bool SetOutputDevice(string active)
        {
            return this.api.Player_SetOutputDevice(active);
        }

        public bool SetVolume(int volume)
        {
            var success = false;
            if (volume >= 0)
            {
                success = this.api.Player_SetVolume((float)volume / 100);
            }

            if (this.api.Player_GetMute())
            {
                success = this.api.Player_SetMute(false);
            }

            return success;
        }

        public string GetPlayState()
        {
            return this.api.Player_GetPlayState().ToString().ToLower();
        }

        public bool SetShuffleState(ShuffleState state)
        {
            var success = false;
            switch (state)
            {
                case ShuffleState.autodj:
                    success = this.api.Player_StartAutoDj();
                    break;
                case ShuffleState.off:
                    success = this.api.Player_SetShuffle(false);
                    break;
                case ShuffleState.shuffle:
                    success = this.api.Player_SetShuffle(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            return success;
        }

        public ShuffleState GetShuffleState()
        {
            ShuffleState state;
            if (this.api.Player_GetAutoDjEnabled())
            {
                state = ShuffleState.autodj;
            }
            else
            {
                var shuffleEnabled = this.api.Player_GetShuffle();
                state = shuffleEnabled ? ShuffleState.shuffle : ShuffleState.off;
            }
            return state;
        }

        public bool SetRepeatState(ApiRepeatMode mode)
        {
            var success = false;
            Plugin.RepeatMode repeatMode;

            switch (mode)
            {
                case ApiRepeatMode.all:
                    repeatMode = Plugin.RepeatMode.All;
                    break;
                case ApiRepeatMode.none:
                    repeatMode = Plugin.RepeatMode.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
            success = this.api.Player_SetRepeat(repeatMode);

            return success;
        }

        public bool StartPlayback()
        {
            var success = false;
            var playState = this.api.Player_GetPlayState();
            if (playState != Plugin.PlayState.Playing)
            {
                success = this.api.Player_PlayPause();
            }
            return success;
        }

        public bool PausePlayback()
        {
            var success = false;
            var playState = this.api.Player_GetPlayState();
            if (playState == Plugin.PlayState.Playing)
            {
                success = this.api.Player_PlayPause();
            }
            return success;
        }

        public bool ChangeAutoDj(bool enabled)
        {
            return enabled ? this.api.Player_StartAutoDj() : this.api.Player_EndAutoDj();
        }

        public bool GetAutoDjState()
        {
            return this.api.Player_GetAutoDjEnabled();
        }

        public string GetRepeatState()
        {
            var repeatState = this.api.Player_GetRepeat().ToString();
            return repeatState.ToLower();
        }

        public bool SetScrobbleState(bool enabled)
        {
            return this.api.Player_SetScrobbleEnabled(enabled);
        }

        public bool GetScrobbleState()
        {
            return this.api.Player_GetScrobbleEnabled();
        }

        public bool GetMuteState()
        {
            return this.api.Player_GetMute();
        }

        public bool SetMute(bool enabled)
        {
            return this.api.Player_SetMute(enabled);
        }

        public bool StopPlayback()
        {
            return this.api.Player_Stop();
        }

        public bool ToggleShuffle()
        {
            var success = false;
            var shuffleEnabled = this.api.Player_GetShuffle();
            var autoDjEnabled = this.api.Player_GetAutoDjEnabled();

            if (shuffleEnabled && !autoDjEnabled)
            {
                success = this.api.Player_StartAutoDj();
            }
            else if (autoDjEnabled)
            {
                success = this.api.Player_EndAutoDj();
            }
            else
            {
                success = this.api.Player_SetShuffle(true);
            }

            return success;
        }
    }
}