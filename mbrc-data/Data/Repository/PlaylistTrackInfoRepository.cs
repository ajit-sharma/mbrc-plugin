using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using MusicBeeRemote.Data.Entities;
using MusicBeeRemote.Data.Extensions;
using MusicBeeRemote.Data.Repository.Interfaces;

namespace MusicBeeRemote.Data.Repository
{
    public class PlaylistTrackInfoRepository : GenericRepository<PlaylistTrackInfo>, IPlaylistTrackInfoRepository
    {
        public PlaylistTrackInfoRepository(DatabaseManager manager) : base(manager)
        {
        }

        public IList<int> GetAllIds()
        {
            using (var db = new LiteDatabase(Manager.GetDatabaseFile()))
            {
                var collection = db.GetCollection<PlaylistTrackInfo>(Table());
                return collection.FindAll().Select(info => info.Id).ToList();
            }
        }
       
        public IList<PlaylistTrackInfo> GetTracksForPlaylist(int id)
        {
            using (var db = new LiteDatabase(Manager.GetDatabaseFile()))
            {
                var infoCollection = db.GetCollection<PlaylistTrackInfo>(Table());
                var tracks = db.GetCollection<PlaylistTrack>(typeof(PlaylistTrack).Name.ToLower());
                return tracks.Find(track => track.PlaylistId == id)
                    .Select(track => { return infoCollection.FindOne(trackInfo => trackInfo.Id == track.TrackInfoId); })
                    .OrderBy(info => info.Position)
                    .ToList();
            }
        }

        public int SoftDeleteUnused(IList<int> unused)
        {
            using (var db = new LiteDatabase(Manager.GetDatabaseFile()))
            {
                using (var transaction = db.BeginTrans())
                {
                    var epoch = DateTime.Now.ToUnixTime();

                    var collection = db.GetCollection<PlaylistTrackInfo>(Table());
                    var updated = collection.Find(info => unused.Contains((int) info.Id))
                        .Select(info =>
                        {
                            info.DateDeleted = epoch;
                            return collection.Update(info);
                        }).Count(success => success);
                    transaction.Commit();
                    return updated;
                }
            }
        }
    }
}