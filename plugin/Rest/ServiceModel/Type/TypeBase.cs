#region Dependencies

using System;
using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
	/// <summary>
	///     The <c>base</c> of all the POCOs stored in the database.
	/// </summary>
	[DataContract(Name = "typeBase")]
	public class TypeBase
	{
		/// <summary>
		///     The identity of the entry stored in the database.
		/// </summary>
		[DataMember(Name = "id")]
		[AutoIncrement]
		public long Id { get; set; }

		/// <summary>
		///     The Date the entry was added. It defaults in the UTC <see cref="DateTime" />
		///     of the <see langword="object" /> creation. Used during sync to determine
		///     the newer additions.
		/// </summary>
		[DataMember(Name = "dateAdded")]
		public DateTime DateAdded { get; set; } = DateTime.UtcNow;

		/// <summary>
		///     The Date the entry was last updated. It defaults in the UTC <see cref="DateTime" />
		///     of the <see langword="object" /> creation. Used during sync to figure out the modified entries.
		/// </summary>
		[DataMember(Name = "dateUpdated")]
		public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
	}
}