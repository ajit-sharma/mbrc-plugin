using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MusicBeePlugin.AndroidRemote.Error;
using Encoder = System.Drawing.Imaging.Encoder;

namespace MusicBeePlugin.AndroidRemote.Utilities
{
    public class Utilities
    {
        private static SHA1Managed sha1 = new SHA1Managed();
        private static byte[] hash = new byte[20];

        public static string Sha1Hash(string value)
        {
            var mHash = new String('0', 40);
            if (!String.IsNullOrEmpty(value))
            {
                hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(value));
                var sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    var hex = b.ToString("x2");
                    sb.Append(hex);
                }
                mHash = sb.ToString();
            }

            return mHash;
        }

        public static string Sha1Hash(FileStream fs)
        {
            hash = sha1.ComputeHash(fs);
            var sb = new StringBuilder();
            foreach (byte b in hash)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString();
         
        }

        public static string GetCachedImage(string coverHash)
        {
            string base64 = String.Empty;
            try
            {
                var directory = Settings.UserSettings.Instance.StoragePath + @"cache\cover\";
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
#if DEBUG
                ErrorHandler.LogError(e);
#endif
            }
            return base64;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        /// <summary>
        /// Given a locally stored artist image it resizes and caches the artist image.
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
                var directory = Settings.UserSettings.Instance.StoragePath + @"cache\artist\";
                var filepath = string.Empty;

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fs = new FileStream(url, FileMode.Open, FileAccess.Read))
                {
                    hash = Sha1Hash(fs);
                    filepath = directory + hash;
                    if (File.Exists(filepath))
                    {
                        return hash;
                    }

                    using (Image albumCover = Image.FromStream(fs, true)) { 
                    
                        int sourceWidth = albumCover.Width;
                        int sourceHeight = albumCover.Height;
                        int destWidth = sourceWidth;
                        int destHeight = sourceHeight;

                        if (sourceWidth > width || sourceHeight > height)
                        {
                            float nPercentW = (width / (float)sourceWidth);
                            float nPercentH = (height / (float)sourceHeight);

                            var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;
                            destWidth = (int)(sourceWidth * nPercent);
                            destHeight = (int)(sourceHeight * nPercent);
                        }

                        using (var bmp = new Bitmap(destWidth, destHeight))
                        {
                            Graphics graph = Graphics.FromImage(bmp);
                            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graph.DrawImage(albumCover, 0, 0, destWidth, destHeight);
                            graph.Dispose();

                            ImageCodecInfo mInfo = GetEncoder(ImageFormat.Jpeg);
                            Encoder mEncoder = Encoder.Quality;
                            EncoderParameters mParams = new EncoderParameters(1);
                            EncoderParameter mParam = new EncoderParameter(mEncoder, 80L);
                            mParams.Param[0] = mParam;
                            bmp.Save(filepath, mInfo, mParams);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                ErrorHandler.LogError(ex);
#endif
            }
            return hash; 
        }

        public static string CacheImage(string base64, int width = 400, int height = 400)
        {
            var hash = Sha1Hash(base64);
            try
            {
                var directory = Settings.UserSettings.Instance.StoragePath + @"cache\cover\";
                var filepath = directory + hash;
                if (String.IsNullOrEmpty(base64))
                {
                    return hash;
                }

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(filepath))
                {
                    return hash;
                }
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
                using (Image albumCover = Image.FromStream(ms, true))
                {
                    ms.Flush();
                    int sourceWidth = albumCover.Width;
                    int sourceHeight = albumCover.Height;
                    int destWidth = sourceWidth;
                    int destHeight = sourceHeight;

                    if (sourceWidth > width || sourceHeight > height)
                    {
                        float nPercentW = (width/(float) sourceWidth);
                        float nPercentH = (height/(float) sourceHeight);

                        var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;
                        destWidth = (int) (sourceWidth*nPercent);
                        destHeight = (int) (sourceHeight*nPercent);
                    } 

                    using (var bmp = new Bitmap(destWidth, destHeight))
                    {
                        Graphics graph = Graphics.FromImage(bmp);
                        graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graph.DrawImage(albumCover, 0, 0, destWidth, destHeight);
                        graph.Dispose();

                        ImageCodecInfo mInfo = GetEncoder(ImageFormat.Jpeg);
                        Encoder mEncoder = Encoder.Quality;
                        EncoderParameters mParams = new EncoderParameters(1);
                        EncoderParameter mParam = new EncoderParameter(mEncoder, 80L);
                        mParams.Param[0] = mParam;
                        bmp.Save(filepath, mInfo, mParams);
                    }
                   
                } 
            }
            catch (Exception ex)
            {
#if DEBUG
                ErrorHandler.LogError(ex);
#endif
            }
            return hash;
        }

        public static string ImageResize(string base64, int width = 300, int height = 300)
        {
            var cover = String.Empty;
            try
            {
                if (String.IsNullOrEmpty(base64))
                {
                    return cover;
                }
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
                using (Image albumCover = Image.FromStream(ms, true))
                {
                    ms.Flush();
                    int sourceWidth = albumCover.Width;
                    int sourceHeight = albumCover.Height;

                    float nPercentW = (width/(float) sourceWidth);
                    float nPercentH = (height/(float) sourceHeight);

                    var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;
                    int destWidth = (int) (sourceWidth*nPercent);
                    int destHeight = (int) (sourceHeight*nPercent);
                    using (var bmp = new Bitmap(destWidth, destHeight))
                    using (var ms2 = new MemoryStream())
                    {
                        Graphics graph = Graphics.FromImage(bmp);
                        graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graph.DrawImage(albumCover, 0, 0, destWidth, destHeight);
                        graph.Dispose();

                        ImageCodecInfo mInfo = GetEncoder(ImageFormat.Jpeg);
                        Encoder mEncoder = Encoder.Quality;
                        EncoderParameters mParams = new EncoderParameters(1);
                        EncoderParameter mParam = new EncoderParameter(mEncoder, 80L);
                        mParams.Param[0] = mParam;

                        bmp.Save(ms2, mInfo, mParams);
                        cover = Convert.ToBase64String(ms2.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                ErrorHandler.LogError(ex);
#endif
            }
            return cover;
        }
    }
}