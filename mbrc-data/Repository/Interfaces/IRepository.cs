namespace MusicBeeRemoteData.Repository.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// The interface provides a base for the basic actions of a repository.
    /// </summary>
    /// <typeparam name="T">The type of data the repository handles</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Deletes an item from the repository and returns the number of the rows affected.
        /// This method completely removes the item from the repository. If you just want
        /// to mark it as deleted check the <see cref="SoftDelete"/> method.
        /// </summary>
        /// <param name="t">
        /// The item to be deleted.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> number of rows affected.
        /// </returns>
        int Delete(IList<T> t);
                
        /// <summary>
        /// Marks the items as Deleted (Should update the DateDeleted property),
        /// but keeps them in the database.
        /// </summary>
        /// <param name="t">The items to be soft deleted.</param>
        /// <returns>The number of rows affected.</returns>
        int SoftDelete(IList<T> t);

        /// <summary>
        /// Gets all the items stored in the repository.
        /// </summary>
        /// <returns>A list of all the items stored.</returns>
        IList<T> GetAll();

        /// <summary>
        /// Gets all the cached items. Cached are those that have a 0 value to the DateDeleted property.
        /// </summary>
        /// <returns>A list of the cached, non deleted elements in the repository.</returns>
        IList<T> GetCached();

        /// <summary>
        /// Gets all the deleted items (Soft deleted). These items have the DateDeleted property with a 
        /// non zero value. 
        /// </summary>
        /// <returns>A list of the soft deleted items in the repository.</returns>
        IList<T> GetDeleted();

        /// <summary>
        /// Gets an items from the repository by the id.
        /// </summary>
        /// <param name="id">The id of the item.</param>
        /// <returns>The item matching the supplied id</returns>
        T GetById(long id);

        /// <summary>
        /// Gets the count of items stored in the repository.
        /// </summary>
        /// <returns>The number of items stored in the repository.</returns>
        int GetCount();

        /// <summary>
        /// Gets a page of data from the repository.
        /// </summary>
        /// <param name="offset">The offset of the data set.</param>
        /// <param name="limit">The number of elements in the dataset</param>
        /// <returns>A list containing the page's items.</returns>
        IList<T> GetPage(int offset, int limit);
        
        /// <summary>
        /// Same as <see cref="GetPage"/> but takes into consideration the date updated.
        /// This is used to update the data in the client.
        /// </summary>
        /// <param name="offset">The offset of the data set</param>
        /// <param name="limit">The number of elements in the dataset.</param>
        /// <param name="epoch">The unix epoch after which we require data.</param>
        /// <returns>A page (list) of the updated data.</returns>
        IList<T> GetUpdatedPage(int offset, int limit, long epoch);
        
        /// <summary>
        /// Saves an item to the repository. If the item exists it updates the item, if not 
        /// then it inserts the item.
        /// </summary>
        /// <param name="t">The item</param>
        /// <returns>The id of the item inserted/updated in case of success or 0 if it fails.</returns>
        int Save(T t);
        
        /// <summary>
        /// Saves a list of items in the repository.
        /// </summary>
        /// <param name="t">A list of Items</param>
        /// <returns>The number of rows affected.</returns>
        int Save(IList<T> t);
    }
}