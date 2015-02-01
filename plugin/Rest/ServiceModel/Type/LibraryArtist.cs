#region Dependencies

using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
	/// <summary>
	///     This class represents an Artist with the information used in the
	///     cache and API.
	/// </summary>
	[DataContract]
	[Alias("LibraryArtist")]
	public class LibraryArtist : TypeBase
	{
		/// <summary>
		///     Parametrized constructor of the LibraryArtist class. It creates a
		///     new LibraryArtist with the supplied <paramref name="name" />.
		/// </summary>
		/// <param name="name">The name of the artist created.</param>
		public LibraryArtist(string name)
		{
			Name = name;
		}

		/// <summary>
		///     Default constructor of LibraryArtist class.
		/// </summary>
		public LibraryArtist()
		{
		}

		/// <summary>
		///     The name property of the artist. The name should be unique.
		/// </summary>
		[DataMember(Name = "name")]
		[Index(Unique = true)]
		public string Name { get; set; }

		/// <summary>
		///     The genre id property that represents the artist's genre.
		/// </summary>
		[DataMember(Name = "genre")]
		[References(typeof (LibraryGenre))]
		public long GenreId { get; set; }

		/// <summary>
		///     A URL to the place the artist image is stored.
		/// </summary>
		[DataMember(Name = "imageUrl")]
		public string ImageUrl { get; set; }
	}
}