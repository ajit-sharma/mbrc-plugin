using System;
using System.Runtime.Serialization;

namespace MusicBeePlugin.AndroidRemote.Persistance
{
	[DataContract]
	class LastUpdated
	{
		public DateTime PlaylistLastCheck { get; set; }
		public DateTime PlaylistLastChange { get; set; }
	}
}
