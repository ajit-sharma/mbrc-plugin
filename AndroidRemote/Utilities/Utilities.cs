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
            hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(value));
            var sb = new StringBuilder();
            foreach (byte b in hash)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString();
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

                        bmp.Save(ms2, System.Drawing.Imaging.ImageFormat.Png);
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