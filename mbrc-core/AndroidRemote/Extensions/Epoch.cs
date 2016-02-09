namespace MusicBeePlugin.AndroidRemote.Extensions
{
    using System;

    public static class Epoch
    {
        public static DateTime FromUnixTime(this long epoch)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return date.AddSeconds(epoch);
        }

        public static long ToUnixTime(this DateTime date)
        {
            var epochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epochDate).TotalSeconds);
        }
    }
}