using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Networking;

namespace MusicBeePlugin.AndroidRemote.Model
{
    using System;
    using System.Security;
    using System.Text.RegularExpressions;
    using Error;
    using Events;

    internal class LyricCoverModel
    {
        /** Singleton **/
        private static readonly LyricCoverModel Model = new LyricCoverModel();

        private string _xHash;
        private string _lyrics;

        public static LyricCoverModel Instance
        {
            get { return Model; }
        }

        private LyricCoverModel()
        {
            
        }

        public void SetCover(string base64)
        {
            var hash = Utilities.Utilities.Sha1Hash(base64);
            
            if (_xHash != null && _xHash.Equals(hash))
            {
                return;
            }

            Cover = String.IsNullOrEmpty(base64)
                ? String.Empty
                : Utilities.Utilities.ImageResize(base64);
            _xHash = hash;
            
            EventBus.FireEvent(
                    new MessageEvent(EventType.ReplyAvailable,
                        new SocketMessage(Constants.NowPlayingCover, Constants.Message, Cover).toJsonString()));
            
        }

        public string Cover { get; private set; }

        public string Lyrics
        {
            set
            {
                try
                {
                    var lStr = value.Trim();
                    if (lStr.Contains("\r\r\n\r\r\n"))
                    {
                        /* Convert new line & empty line to xml safe format */
                        lStr = lStr.Replace("\r\r\n\r\r\n", " \r\n ");
                        lStr = lStr.Replace("\r\r\n", " \n ");
                    }
                    lStr = lStr.Replace("\0", " ");
                    const string pattern = "\\[\\d:\\d{2}.\\d{3}\\] ";
                    var regEx = new Regex(pattern);
                    _lyrics = SecurityElement.Escape(regEx.Replace(lStr, String.Empty));
                }
                catch (Exception ex)
                {
#if DEBUG
                    ErrorHandler.LogError(ex);
#endif
                    _lyrics = String.Empty;
                }
                finally
                {
                    if (!String.IsNullOrEmpty(_lyrics))
                        EventBus.FireEvent(
                            new MessageEvent(EventType.ReplyAvailable,
                                new SocketMessage(Constants.NowPlayingLyrics, Constants.Message, _lyrics).toJsonString()));
                }
            }
            get { return _lyrics; }
        }
    }
}