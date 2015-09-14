using System;
using System.Collections.Generic;
using MusicBeePlugin.AndroidRemote.Data;
using MusicBeePlugin.Rest.ServiceModel.Enum;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.OrmLite;
using ServiceStack.Text;

namespace MusicBeePlugin.Modules
{
	/// <summary>
	/// The base of the modules
	/// </summary>
	public class DataModuleBase
	{
		/// <summary>
		///     <see cref="CacheHelper" />
		/// </summary>
		protected CacheHelper _cHelper;

		/// <summary>
		///     MusicBee API Interface.
		/// </summary>
		protected Plugin.MusicBeeApiInterface _api;


		/// <summary>
		/// Retrieves the changes stored in the database for a class that inherits
		/// from the TypeBase class.
		/// </summary>
		/// <param name="date">The date</param>
		/// <param name="change">The type of change.</param>
		/// <typeparam name="T"><see cref="TypeBase"/> object.</typeparam>
		/// <returns></returns>
		public List<T> GetChangesSince<T>(DateTime date, ChangeType change) where T : TypeBase
		{
			var list = new List<T>();

			using (var db = _cHelper.GetDbConnection())
			{
				switch (change)
				{
					case ChangeType.added:
						list = db.Select<T>(pt => pt.DateAdded >= date.ToUnixTime());
						break;
					case ChangeType.deleted:
						list = db.Select<T>(pt => pt.DateDeleted >= date.ToUnixTime());
						break;
					case ChangeType.updated:
						list = db.Select<T>(pt => pt.DateUpdated >= date.ToUnixTime());
						break;
				    default:
				        throw new ArgumentOutOfRangeException(nameof(change), change, null);
				}
			}

		    return list;
		}
	}
}