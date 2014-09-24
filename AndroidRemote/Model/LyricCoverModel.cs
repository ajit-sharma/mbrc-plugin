#region

using System;
using System.Diagnostics;
using System.Security;
using System.Text.RegularExpressions;
using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Events;
using NLog;
using ServiceStack.Common.Utils;

#endregion

namespace MusicBeePlugin.AndroidRemote.Model
{
    public class LyricCoverModel
    {
        public LyricCoverModel()
        {
            Debug.Write(this.GetId());
        }
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string _lyrics;
        private string _xHash;

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
                    Logger.Debug(ex);
                    _lyrics = String.Empty;
                }
                finally
                {
                    if (!String.IsNullOrEmpty(_lyrics))
                        EventBus.FireEvent(
                            new MessageEvent(EventType.ReplyAvailable,
                                new NotificationMessage(NotificationMessage.LyricsChanged).ToJsonString()));
                }
            }
            get { return _lyrics; }
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
                    new NotificationMessage(NotificationMessage.CoverChanged).ToJsonString()));
        }
    }
}