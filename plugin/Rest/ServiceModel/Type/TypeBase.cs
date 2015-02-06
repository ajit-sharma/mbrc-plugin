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

		/// <summary>
		///     The Date the entry was deleted. As it is expected for an entry that is
		///     active <c>this</c> field should have a <see langword="null" /> value.
		///     The objects are initially soft deleted and periodically a function is
		///     responsible objects that have been deleted for a number of days.
		/// </summary>
		[DataMember(Name = "dateDeleted")]
		public DateTime? DateDeleted { get; set; }
	}
}