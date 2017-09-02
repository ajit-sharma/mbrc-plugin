using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using LiteDB;

namespace MusicBeeRemote.Data
{
    /// <summary>
    /// Class DatabaseProvider.
    /// Is used to handle the library data and cover cache
    /// </summary>
    public class DatabaseManager
    {
        /// <summary>
        /// The full path on the filesystem where the database files resides.
        /// </summary>
        private readonly string _databaseFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        /// <param name="databaseFile">The storage path.</param>
        public DatabaseManager(string databaseFile)
        {
            _databaseFile = databaseFile;
            Configure();
        }

        /// <summary>
        /// Gets a connection to the database.
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseFile() => _databaseFile;

        /// <summary>
        /// Deletes completely the database file.
        /// </summary>
        public void DeleteDatabase()
        {
            if (File.Exists(_databaseFile))
            {
                File.Delete(_databaseFile);
            }
        }

        /// <summary>
        /// Informs the nosql mapper on how to handle conversions of IPAddresses and MAC Addresses.
        /// </summary>
        private static void Configure()
        {
            //Bson serialization for client IP addresses
            BsonMapper.Global.RegisterType(
                ipAddress => ipAddress.ToString(),
                bson => IPAddress.Parse(bson.AsString)
            );

            //Bson serialization for client Physical Addresses
            BsonMapper.Global.RegisterType(
                mac => mac.ToString(),
                bson =>
                {
                    var newAddress = PhysicalAddress.Parse(bson);
                    return PhysicalAddress.None.Equals(newAddress) ? null : newAddress;
                }
            );
        }
    }
}