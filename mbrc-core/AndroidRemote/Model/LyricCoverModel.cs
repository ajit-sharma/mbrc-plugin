namespace MusicBeeRemoteCore.AndroidRemote.Model
{
    using System;
    using System.Security;
    using System.Text.RegularExpressions;

    using MusicBeeRemoteCore.AndroidRemote.Entities;
    using MusicBeeRemoteCore.AndroidRemote.Events;

    using NLog;

    public class LyricCoverModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly EventBus bus;

        private string _lyrics;

        private string _previousCoverHash;

        public LyricCoverModel(EventBus bus)
        {
            this.bus = bus;
        }

        public string Cover { get; private set; }

        public string Lyrics
        {
            get
            {
                return this._lyrics;
            }

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
                    var intermediate = regEx.Replace(lStr, string.Empty);
                    this._lyrics = SecurityElement.Escape(intermediate);
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex);
                    this._lyrics = string.Empty;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(this._lyrics))
                    {
                        var notification = new NotificationMessage(NotificationMessage.LyricsChanged);
                        var @event = new MessageEvent(MessageEvent.Notify, notification.ToJsonString());
                        this.bus.Publish(@event);
                    }
                }
            }
        }

        public void SetCover(string base64)
        {
            var hash = Utilities.Utilities.Sha1Hash(base64);

            if (this._previousCoverHash != null && this._previousCoverHash.Equals(hash))
            {
                return;
            }

            this.Cover = string.IsNullOrEmpty(base64) ? string.Empty : base64;
            this._previousCoverHash = hash;

            var notification = new NotificationMessage(NotificationMessage.CoverChanged);
            var @event = new MessageEvent(MessageEvent.Notify, notification.ToJsonString());
            this.bus.Publish(@event);
        }
    }
}