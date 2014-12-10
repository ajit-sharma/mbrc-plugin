#region

using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Events;
using NLog;
using System;
using System.Security;
using System.Text.RegularExpressions;

#endregion

namespace MusicBeePlugin.AndroidRemote.Model
{
    public class LyricCoverModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string _lyrics;
        private string _previousCoverHash;

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
                    var intermediate = regEx.Replace(lStr, String.Empty);
                    _lyrics = SecurityElement.Escape(intermediate);
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex);
                    _lyrics = String.Empty;
                }
                finally
                {
                    if (!String.IsNullOrEmpty(_lyrics))
                    {
                        var notification = new NotificationMessage(NotificationMessage.LyricsChanged);
                        var @event = new MessageEvent(MessageEvent.Notify, notification.ToJsonString());
                        EventBus.FireEvent(@event);
                    }
                }
            }
            get { return _lyrics; }
        }

        public void SetCover(string base64)
        {
            var hash = Utilities.Utilities.Sha1Hash(base64);

            if (_previousCoverHash != null && _previousCoverHash.Equals(hash))
            {
                return;
            }

            Cover = String.IsNullOrEmpty(base64)
                ? String.Empty
                : Utilities.Utilities.ImageResize(base64);
            _previousCoverHash = hash;

            var notification = new NotificationMessage(NotificationMessage.CoverChanged);
            var @event = new MessageEvent(MessageEvent.Notify, notification.ToJsonString());
            EventBus.FireEvent(@event);
        }
    }
}