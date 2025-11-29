using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TFU.DAL.Interfaces
{
	public interface ICRUDRepository<TDto, TKey>
	{
		/// <summary>
		/// Gets the by identifier.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id">The identifier.</param>
		/// <returns></returns>
		Task<TDto> GetByID(TKey id);

		/// <summary>
		/// Inserts the specified object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The object.</param>
		/// <param name="storedProcedure">The stored procedure.</param>
		/// <param name="saveType">Type of the save.</param>
		/// <param name="userId">The user identifier.</param>
		/// <returns></returns>
		Task<TKey> Insert(TDto obj);

		/// <summary>
		/// Updates the specified object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="storedProcedure"></param>
		/// <returns></returns>
		Task<TKey> Update(TDto obj);

		/// <summary>
		/// Deletes the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="storedProcedure">The stored procedure.</param>
		/// <returns></returns>
		Task<bool> Delete(TKey id);
	}
}
