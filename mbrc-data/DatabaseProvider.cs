using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using LiteDB;
using NLog;
using Logger = NLog.Logger;

namespace MusicBeeRemoteData
{
    /// <summary>
    ///     Class DatabaseProvider.
    ///     Is used to handle the library data and cover cache
    /// </summary>
    public class DatabaseProvider
    {
        private const string DbName = @"cache.db";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _dbFilePath;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseProvider" /> class.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        public DatabaseProvider(string storagePath)
        {
            Configure();
            
            if (!storagePath.EndsWith("\\"))
            {
                storagePath += "\\";
            }

            _dbFilePath = storagePath + DbName;

            try
            {
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }
        }

        /// <summary>
        ///     Gets a connection to the database.
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseFile()
        {
            return _dbFilePath;
        }

        public void ResetDatabase()
        {
            DeleteDatabase();
        }

        public void DeleteDatabase()
        {
            if (File.Exists(_dbFilePath))
            {
                File.Delete(_dbFilePath);
            }
        }

        private void Configure()
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