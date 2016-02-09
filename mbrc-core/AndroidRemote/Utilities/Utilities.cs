namespace MusicBeePlugin.AndroidRemote.Utilities
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using NLog;

    using Encoder = System.Drawing.Imaging.Encoder;

    /// <summary>
    ///     Utilities for hashing and IO based operations
    /// </summary>
    public class Utilities
    {
        private const string CacheArtist = @"cache\artist\";

        private const string CacheCover = @"\cache\cover\";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly SHA1Managed Sha1 = new SHA1Managed();

        private static byte[] _hash = new byte[20];

        /// <summary>
        /// Base path where the files of the plugin are stored.
        /// </summary>
        public static string StoragePath { get; set; }

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
                var directory = Directory(CacheArtist);

                using (var fs = new FileStream(url, FileMode.Open, FileAccess.Read))
                {
                    hash = Sha1Hash(fs);
                    var filepath = directory + hash;
                    if (File.Exists(filepath))
                    {
                        return hash;
                    }

                    StoreResizedStream(fs, filepath, width, height);
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
            }

            return hash;
        }

        /// <summary>
        /// Converts a base64 encoded string to a Stream.
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
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

        /// <summary>
        ///     Reads an image file from the filesystem and returns a stream.
        /// </summary>
        /// <param name="hash">SHA1 hash used as an identifier for the image</param>
        /// <returns></returns>
        public static Stream GetCoverStreamFromCache(string hash)
        {
            var ms = new MemoryStream();
            try
            {
                var directory = StoragePath + CacheCover;
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

        /// <summary>
        ///     Given a base64 encoded image it resizes the image to the specified
        ///     width & height and returns the base64 of the resized image.
        /// </summary>
        /// <param name="base64">The base64 of the original image.</param>
        /// <param name="width">The width of the new image.</param>
        /// <param name="height">The height of the new image.</param>
        /// <returns>System.String. The base64 of the resized image</returns>
        public static string GetResizedBase64(string base64, int width = 300, int height = 300)
        {
            var cover = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(base64))
                {
                    return cover;
                }

                using (var ms = new MemoryStream(Convert.FromBase64String(base64)))
                using (var albumCover = Image.FromStream(ms, true))
                {
                    ms.Flush();
                    var size = CalculateNewSize(width, height, albumCover);
                    using (var bmp = new Bitmap(size.Width, size.Height))
                    using (var ms2 = new MemoryStream())
                    {
                        var graph = Graphics.FromImage(bmp);
                        graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graph.DrawImage(albumCover, 0, 0, size.Width, size.Height);
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

        /// <summary>
        ///     Gets a string value and calculates the sha1 hash of the string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String. The SHA1 hash value of the string</returns>
        public static string Sha1Hash(string value)
        {
            var mHash = new String('0', 40);
            if (string.IsNullOrEmpty(value))
            {
                return mHash;
            }

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
        ///     Opens a <see cref="Stream" /> and calculates the SHA1 hash for the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>System.String. The SHA1 hash value of calculated from the stream contents.</returns>
        public static string Sha1Hash(Stream stream)
        {
            _hash = Sha1.ComputeHash(stream);
            var sb = new StringBuilder();
            foreach (var hex in _hash.Select(b => b.ToString("x2")))
            {
                sb.Append(hex);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Resizes the cover and stores it to cache and returns the hash code
        ///     for the image.
        /// </summary>
        /// <param name="url">
        ///     The path where the original cover is stored.
        /// </param>
        /// <param name="width">The width of the cached image.</param>
        /// <param name="height">The height of the cached image.</param>
        /// <returns>
        ///     System.String. The SHA1 hash representing the image
        /// </returns>
        public static string StoreCoverToCache(string url, int width = 400, int height = 400)
        {
            var hash = string.Empty;
            if (string.IsNullOrEmpty(url))
            {
                return hash;
            }

            try
            {
                var directory = Directory(CacheCover);

                using (var fs = new FileStream(url, FileMode.Open, FileAccess.Read))
                {
                    hash = Sha1Hash(fs);
                    var filepath = directory + hash;
                    if (File.Exists(filepath))
                    {
                        return hash;
                    }

                    if (!StoreResizedStream(fs, filepath, width, height))
                    {
                        hash = new string('0', 40);
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
        /// Resizes an image to specific dimensions and stores it to the plugin Cover cache.
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="width">The max width of the new image</param>
        /// <param name="height">The max height of the new image</param>
        /// <returns>String SHA1 hash of the image.</returns>
        public static string StoreCoverToCache(byte[] imageData, int width = 400, int height = 400)
        {
            var hash = string.Empty;
            if (imageData == null)
            {
                return hash;
            }

            var directory = Directory(CacheCover);

            using (var ms = new MemoryStream(imageData))
            {
                hash = Sha1Hash(ms);
                var filepath = directory + hash;
                if (File.Exists(filepath))
                {
                    return hash;
                }

                StoreResizedStream(ms, filepath, width, height);
            }

            return hash;
        }

        /// <summary>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="albumCover"></param>
        /// <returns></returns>
        private static Size CalculateNewSize(int width, int height, Image albumCover)
        {
            var sourceWidth = albumCover.Width;
            var sourceHeight = albumCover.Height;
            var newWidth = sourceWidth;
            var newHeight = sourceHeight;

            if (sourceWidth <= width && sourceHeight <= height)
            {
                return new Size(newWidth, newHeight);
            }

            var nPercentW = width / (float)sourceWidth;
            var nPercentH = height / (float)sourceHeight;

            var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;
            newWidth = (int)(sourceWidth * nPercent);
            newHeight = (int)(sourceHeight * nPercent);
            return new Size(newWidth, newHeight);
        }

        /// <summary>
        ///     Given a <paramref name="subdirectory" /> it will return the full
        ///     path where the files will be stored. If the directory does not exist
        ///     it will create the directory.
        /// </summary>
        /// <param name="subdirectory">
        ///     The specific subdirectory see
        ///     <see cref="CacheArtist" /> and <see cref="CacheCover" />
        /// </param>
        /// <returns>
        ///     <see cref="string" /> The full path to to the cache
        /// </returns>
        private static string Directory(string subdirectory)
        {
            var directory = StoragePath + subdirectory;

            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            return directory;
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
        ///     Given an <paramref name="filepath" />, an <paramref name="image" />
        ///     and the dimensions it will store the <see cref="Image" /> to the
        ///     specified path in the filesystem.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="image"></param>
        private static void StoreImage(string filepath, int width, int height, Image image)
        {
            using (var bmp = new Bitmap(width, height))
            {
                var graph = Graphics.FromImage(bmp);
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.DrawImage(image, 0, 0, width, height);
                graph.Dispose();

                var mInfo = GetEncoder(ImageFormat.Jpeg);
                var mEncoder = Encoder.Quality;
                var mParams = new EncoderParameters(1);
                var mParam = new EncoderParameter(mEncoder, 80L);
                mParams.Param[0] = mParam;
                bmp.Save(filepath, mInfo, mParams);
            }
        }

        /// <summary>
        ///     Given a Stream that is supposedly an image that function will try to
        ///     resize the image to the supplied <paramref name="width" /> and
        ///     <paramref name="height" /> . If the image is not square the the
        ///     dimensions represent the largest size.
        /// </summary>
        /// <param name="stream">A stream containing an image.</param>
        /// <param name="filepath">The path where the file will be saved</param>
        /// <param name="width">The max width of the file saved</param>
        /// <param name="height">The max height of the file saved</param>
        private static bool StoreResizedStream(Stream stream, string filepath, int width = 400, int height = 400)
        {
            var success = true;
            try
            {
                var albumCover = Image.FromStream(stream, false, true);
                var size = CalculateNewSize(width, height, albumCover);
                StoreImage(filepath, size.Width, size.Height, albumCover);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex);
                success = false;
            }

            return success;
        }
    }
}