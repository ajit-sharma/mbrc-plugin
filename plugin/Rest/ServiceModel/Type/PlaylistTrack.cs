#region Dependencies

using System;
using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
	[DataContract]
	public class PlaylistTrack : IEquatable<PlaylistTrack>
	{
		[AutoIncrement]
		[DataMember(Name = "id")]
		public long Id { get; set; }

		[DataMember(Name = "trackInfoId")]
		[References(typeof (PlaylistTrackInfo))]
		public long TrackInfoId { get; set; }

		[DataMember(Name = "playlistId")]
		[References(typeof (Playlist))]
		public long PlaylistId { get; set; }

		[DataMember(Name = "position")]
		public int Position { get; set; }

		public bool Equals(PlaylistTrack other)
		{
			return TrackInfoId == other.TrackInfoId
			       && PlaylistId == other.PlaylistId
			       && Position == other.Position;
		}
	}
}