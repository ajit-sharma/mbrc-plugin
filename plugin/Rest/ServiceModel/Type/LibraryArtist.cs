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
		/// Backing field for the <see cref="Name"/> property
		/// </summary>
		private string _name;

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
		public string Name
		{
			get { return _name; }
			set { _name = string.IsNullOrEmpty(value) ? "[Empty]" : value; }
		}

		/// <summary>
		///     The genre id property that represents the artist's genre.
		/// </summary>
		[DataMember(Name = "genre")]
		[References(typeof (LibraryGenre))]
		public long GenreId { get; set; }

		/// <summary>
		///     A URL to the place the artist image is stored.
		/// </summary>
		[DataMember(Name = "image_url")]
		public string ImageUrl { get; set; }
	}
}