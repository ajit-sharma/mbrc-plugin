using System.IO;
using System.Security.Cryptography;
using System.Text;

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
    }
}