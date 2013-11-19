using System.Security.Cryptography;
using System.Text;

namespace MusicBeePlugin.AndroidRemote.Utilities
{
    public class Utilities
    {
        public static string Sha1Hash(string value)
        {
            SHA1Managed sha1 = new SHA1Managed();
            byte[] bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(value));
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString();
        }
    }
}