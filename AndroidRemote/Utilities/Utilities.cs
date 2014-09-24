#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MusicBeePlugin.AndroidRemote.Settings;
using NLog;
using Encoder = System.Drawing.Imaging.Encoder;

#endregion

namespace MusicBeePlugin.AndroidRemote.Utilities
{
    public class Utilities
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly SHA1Managed Sha1 = new SHA1Managed();
        private static byte[] _hash = new byte[20];

        /// <summary>
        ///     Gets a string value and calculates the sha1 hash of the string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String. The SHA1 hash value of the string</returns>
        public static string Sha1Hash(string value)
        {
            var mHash = new String('0', 40);
            if (String.IsNullOrEmpty(value)) return mHash;
            _hash = Sha1.ComputeHash(Encoding.UTF8.GetBytes(value));
            var sb = new StringBuilder();
            foreach (var hex in _hash.Select(b => b.ToString("x2")))
            {
                sb.Append(hex);
            }
            mHash = sb.ToString();
            return mHash;
        }

        /// <summary>
        ///     Opens a file stream and calculates the sha1 hash for the stream.
        /// </summary>
        /// <param name="fs">The fs.</param>
        /// <returns>System.String. The SHA1 hash value of the filestream</returns>
        public static string Sha1Hash(FileStream fs)
        {
            _hash = Sha1.ComputeHash(fs);
            var sb = new StringBuilder();
            foreach (var hex in _hash.Select(b => b.ToString("x2")))
            {
                sb.Append(hex);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Given a sha1 hash gets the Base64 encoded string of the cover image data.
        /// </summary>
        /// <param name="coverHash">The SHA1 hash representing the image</param>
        /// <returns>System.String. The image data encoded in Base64</returns>
        public static string GetCachedCoverBase64(string coverHash)
        {
            var base64 = String.Empty;
            try
            {
                var directory = UserSettings.Instance.StoragePath + @"cache\cover\";
                var filepath = directory + coverHash;
                using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, (int) fs.Length);
                    base64 = Convert.ToBase64String(buffer);
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }
            return base64;
        }

        /// <summary>
        ///     Gets the encoder.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>ImageCodecInfo.</returns>
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        /// <summary>
        ///     Given a locally stored artist image it resizes and caches the artist image.
        /// </summary>
        /// <param name="url">The local path where the image is stored locally</param>
        /// <param name="artist">The artist.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>System.String.</returns>
        public static string CacheArtistImage(string url, string artist, int width = 600, int height = 600)
        {
            var hash = new string('0', 40);
            try
            {
                var directory = UserSettings.Instance.StoragePath + @"cache\artist\";

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fs = new FileStream(url, FileMode.Open, FileAccess.Read))
                {
                    hash = Sha1Hash(fs);
                    var filepath = directory + hash;
                    if (File.Exists(filepath))
                    {
                        return hash;
                    }

                    using (var albumCover = Image.FromStream(fs, true))
                    {
                        var sourceWidth = albumCover.Width;
                        var sourceHeight = albumCover.Height;
                        var destWidth = sourceWidth;
                        var destHeight = sourceHeight;

                        if (sourceWidth > width || sourceHeight > height)
                        {
                            var nPercentW = (width/(float) sourceWidth);
                            var nPercentH = (height/(float) sourceHeight);

                            var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;
                            destWidth = (int) (sourceWidth*nPercent);
                            destHeight = (int) (sourceHeight*nPercent);
                        }

                        using (var bmp = new Bitmap(destWidth, destHeight))
                        {
                            var graph = Graphics.FromImage(bmp);
                            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graph.DrawImage(albumCover, 0, 0, destWidth, destHeight);
                            graph.Dispose();

                            var mInfo = GetEncoder(ImageFormat.Jpeg);
                            var mEncoder = Encoder.Quality;
                            var mParams = new EncoderParameters(1);
                            var mParam = new EncoderParameter(mEncoder, 80L);
                            mParams.Param[0] = mParam;
                            bmp.Save(filepath, mInfo, mParams);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
            }
            return hash;
        }

        /// <summary>
        ///     Resizes the cover and stores it to cache and returns the hash code for the image.
        /// </summary>
        /// <param name="url">The path where the original cover is stored.</param>
        /// <param name="width">The width of the cached image.</param>
        /// <param name="height">The height of the cached image.</param>
        /// <returns>System.String. The SHA1 hash representing the image</returns>
        public static string StoreCoverToCache(string url, int width = 400, int height = 400)
        {
            var hash = string.Empty;
            if (String.IsNullOrEmpty(url))
            {
                return hash;
            }
            try
            {
                var directory = UserSettings.Instance.StoragePath + @"cache\cover\";

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fs = new FileStream(url, FileMode.Open, FileAccess.Read))
                {
                    hash = Sha1Hash(fs);
                    var filepath = directory + hash;
                    if (File.Exists(filepath))
                    {
                        return hash;
                    }

                    using (var albumCover = Image.FromStream(fs, true, true))
                    {
                        var sourceWidth = albumCover.Width;
                        var sourceHeight = albumCover.Height;
                        var destWidth = sourceWidth;
                        var destHeight = sourceHeight;

                        if (sourceWidth > width || sourceHeight > height)
                        {
                            var nPercentW = (width/(float) sourceWidth);
                            var nPercentH = (height/(float) sourceHeight);

                            var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;
                            destWidth = (int) (sourceWidth*nPercent);
                            destHeight = (int) (sourceHeight*nPercent);
                        }

                        using (var bmp = new Bitmap(destWidth, destHeight))
                        {
                            var graph = Graphics.FromImage(bmp);
                            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graph.DrawImage(albumCover, 0, 0, destWidth, destHeight);
                            graph.Dispose();

                            var mInfo = GetEncoder(ImageFormat.Jpeg);
                            var mEncoder = Encoder.Quality;
                            var mParams = new EncoderParameters(1);
                            var mParam = new EncoderParameter(mEncoder, 80L);
                            mParams.Param[0] = mParam;
                            bmp.Save(filepath, mInfo, mParams);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
            }
            return hash;
        }

        /// <summary>
        ///     Given a base64 encoded image it resizes the image to the specified width & height and returns
        ///     the base64 of the resized image.
        /// </summary>
        /// <param name="base64">The base64 of the original image.</param>
        /// <param name="width">The width of the new image.</param>
        /// <param name="height">The height of the new image.</param>
        /// <returns>System.String. The base64 of the resized image</returns>
        public static string ImageResize(string base64, int width = 300, int height = 300)
        {
            var cover = String.Empty;
            try
            {
                if (String.IsNullOrEmpty(base64))
                {
                    return cover;
                }
                using (var ms = new MemoryStream(Convert.FromBase64String(base64)))
                using (var albumCover = Image.FromStream(ms, true))
                {
                    ms.Flush();
                    var sourceWidth = albumCover.Width;
                    var sourceHeight = albumCover.Height;

                    var nPercentW = (width/(float) sourceWidth);
                    var nPercentH = (height/(float) sourceHeight);

                    var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;
                    var destWidth = (int) (sourceWidth*nPercent);
                    var destHeight = (int) (sourceHeight*nPercent);
                    using (var bmp = new Bitmap(destWidth, destHeight))
                    using (var ms2 = new MemoryStream())
                    {
                        var graph = Graphics.FromImage(bmp);
                        graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graph.DrawImage(albumCover, 0, 0, destWidth, destHeight);
                        graph.Dispose();

                        var mInfo = GetEncoder(ImageFormat.Jpeg);
                        var mEncoder = Encoder.Quality;
                        var mParams = new EncoderParameters(1);
                        var mParam = new EncoderParameter(mEncoder, 80L);
                        mParams.Param[0] = mParam;

                        bmp.Save(ms2, mInfo, mParams);
                        cover = Convert.ToBase64String(ms2.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
            }
            return cover;
        }

        public static Stream GetCoverStreamFromCache(string hash)
        {
            var ms = new MemoryStream();
            try
            {
                var directory = UserSettings.Instance.StoragePath + @"cache\cover\";
                var filepath = directory + hash;
                using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, (int)fs.Length);
                    using (var image = Image.FromStream(fs))
                    {
                        image.Save(ms, ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
            }
            return ms;
        }

        public static Stream GetCoverStreamFromBase64(string base64)
        {
            var ms = new MemoryStream();
            try
            {
                ms = new MemoryStream(Convert.FromBase64String(base64));
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);   
            }
            return ms;
        }
    }
}