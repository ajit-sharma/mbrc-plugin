namespace MusicBeeRemoteData.Extensions
{
    using System;

    /// <summary>
    /// <see cref="DateTime"/> to epoch conversion extension
    /// </summary>
    public static class Epoch
    {
        /// <summary>
        /// Gets a <see cref="DateTime"/> from a Unix epoch (in seconds)
        /// </summary>
        /// <param name="epoch">The unix epoch in seconds</param>
        /// <returns> The <see cref="DateTime"/> representation of the epoch</returns>
        public static DateTime FromUnixTime(this long epoch)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return date.AddSeconds(epoch);
        }

        /// <summary>
        /// Gets a unix epoch in seconds from a <see cref="DateTime"/>
        /// </summary>
        /// <param name="date">The original <see cref="DateTime"/></param>
        /// <returns>The unix epoch in seconds</returns>
        public static long ToUnixTime(this DateTime date)
        {
            var epochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epochDate).TotalSeconds);
        }
    }
}