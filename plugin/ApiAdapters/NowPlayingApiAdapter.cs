using System.Linq;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;

namespace MusicBeePlugin.ApiAdapters
{
    using System.Collections.Generic;

    class NowPlayingApiAdapter : INowPlayingApiAdapter
    {
        private readonly Plugin.MusicBeeApiInterface _api;

        public NowPlayingApiAdapter(Plugin.MusicBeeApiInterface api)
        {
            _api = api;
        }

        public bool MoveTrack(int from, int to)
        {
            int[] aFrom = {from};
            int dIn;
            if (from > to)
            {
                dIn = to - 1;
            }
            else
            {
                dIn = to;
            }

            return _api.NowPlayingList_MoveFiles(aFrom, dIn);
        }

        public bool PlayIndex(int index)
        {
            var success = false;
            string[] tracks;
            _api.NowPlayingList_QueryFilesEx(null, out tracks);

            if (index >= 0 || index < tracks.Length)
            {
                success = _api.NowPlayingList_PlayNow(tracks[index - 1]);
            }

            return success;
        }

        public bool PlayPath(string path)
        {
            return _api.NowPlayingList_PlayNow(path);
        }

        public bool RemoveIndex(int index)
        {
            return _api.NowPlayingList_RemoveAt(index);
        }

        public IEnumerable<NowPlaying> GetTracks(int offset = 0, int limit = 4000)
        {
            string[] tracks;
            _api.NowPlayingList_QueryFilesEx(null, out tracks);

            return tracks.Select((path, position) =>
            {
                var artist = _api.Library_GetFileTag(path, Plugin.MetaDataType.Artist);
                var title = _api.Library_GetFileTag(path, Plugin.MetaDataType.TrackTitle);

                if (string.IsNullOrEmpty(title))
                {
                    var index = path.LastIndexOf('\\');
                    title = path.Substring(index + 1);
                }

                return new NowPlaying
                {
                    Artist = string.IsNullOrEmpty(artist) ? "Unknown Artist" : artist,
                    Title = title,
                    Position = position + 1,
                    Path = path
                };
            }).ToList();
        }
    }
}