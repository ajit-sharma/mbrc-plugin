#region Dependencies

using System;
using System.Runtime.Serialization;

#endregion

namespace MusicBeePlugin.AndroidRemote.Persistence
{
	/// <summary>
	///     Keeps the dates of the most recent changes in the database.
	///     Used for the sync functionality.
	/// </summary>
	[DataContract]
	internal class LastUpdated
	{
		/// <summary>
		///     The most recent update for the playlists table.
		/// </summary>
		public DateTime PlaylistsUpdated { get; set; }

		/// <summary>
		///     The most recent update for the artists table.
		/// </summary>
		public DateTime ArtistsUpdated { get; set; }

		/// <summary>
		///     The most recent update for the albums table.
		/// </summary>
		public DateTime AlbumsUpdated { get; set; }

		/// <summary>
		///     The most recent update for the genres table.
		/// </summary>
		public DateTime GenresUpdated { get; set; }

		/// <summary>
		///     The most recent update for the tracks table.
		/// </summary>
		public DateTime TracksUpdated { get; set; }

		/// <summary>
		///     All the entries before this Date have been purged from the Database.
		/// </summary>
		public DateTime DeleteThreshold { get; set; }
	}
}