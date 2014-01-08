using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MusicBeePlugin.AndroidRemote.Error;

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

        public static string CacheImage(string base64, int width = 400, int height = 400)
        {
            var hash = Sha1Hash(base64);
            try
            {
                var directory = Settings.UserSettings.Instance.StoragePath + @"cache\";
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
                        
                        bmp.Save(filepath, System.Drawing.Imaging.ImageFormat.Jpeg);
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

                        bmp.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
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