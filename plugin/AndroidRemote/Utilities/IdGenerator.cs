namespace MusicBeePlugin.AndroidRemote.Utilities
{
    using System.Security.Cryptography;
    using System.Text;

    internal class IdGenerator
    {
        public static string GetUniqueKey()
        {
            const int maxSize = 8;
            const string a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var chars = a.ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(maxSize);
            foreach (var b in data)
            {
                result.Append(chars[b%(chars.Length - 1)]);
            }
            return result.ToString();
        }
    }
}