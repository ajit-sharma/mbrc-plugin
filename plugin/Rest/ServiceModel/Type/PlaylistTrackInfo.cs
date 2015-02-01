#region Dependencies

using System;
using System.Runtime.Serialization;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
	/// <summary>
	///     The info of a <see cref="PlaylistTrack" />.
	///     The info are stored seperately to avoid duplication since a track can appear to multiple playlists
	/// </summary>
	[DataContract]
	public class PlaylistTrackInfo : TypeBase, IEquatable<PlaylistTrackInfo>
	{
		/// <summary>
		///     The path of the track in the filesystem.
		/// </summary>
		[DataMember(Name = "path")]
		public string Path { get; set; }

		/// <summary>
		///     The artist performing the track.
		/// </summary>
		[DataMember(Name = "artist")]
		public string Artist { get; set; }

		/// <summary>
		///     The title of the track.
		/// </summary>
		[DataMember(Name = "title")]
		public string Title { get; set; }

		/// <summary>
		///     Checks if an <paramref name="other" /> track is equal to this track. For
		///     two tracks to be equal their Path properties must be equal.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(PlaylistTrackInfo other)
		{
			return Path.Equals(other.Path);
		}
	}
}