using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TFU.Common.Models;

namespace TFU.DAL.Interfaces
{
	public interface ITFURepository
	{
		/// <summary>
		/// Gets all.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="storedProcedure">The stored procedure.</param>
		/// <returns></returns>
		Task<IEnumerable<T>> GetAll<T>(string storedProcedure);

		/// <summary>
		/// Gets the list paging.
		/// </summary>
		/// <typeparam name="T1">The type of the 1.</typeparam>
		/// <param name="pagingModel">The paging model.</param>
		/// <param name="storedProcedure">The stored procedure.</param>
		/// <returns></returns>
		Task<IEnumerable<T>> GetListPaging<T>(PagingModel pagingModel, string storedProcedure);

		/// <summary>
		/// Gets the list paging.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pagingModel">The paging model.</param>
		/// <param name="storedProcedure">The stored procedure.</param>
		/// <returns></returns> 
		Task<List<T1>> GetListPaging<T1, T2>(PagingModel pagingModel, string storedProcedure, List<KeyValuePair<string, string>> sqlParams = null); 

		/// <summary>
		/// Gets the dropdown.
		/// </summary>
		/// <param name="storedProcedure">The stored procedure.</param>
		/// <returns></returns>
		Task<IEnumerable<Dropdown>> GetDropdown(string storedProcedure);

		/// <summary>
		/// Gets the by identifier.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id">The identifier.</param>
		/// <param name="storedProcedure">The stored procedure.</param>
		/// <returns></returns>
		Task<T> GetByID<T>(int id, string storedProcedure);

		/// <summary>
		/// Saves the specified object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The object.</param>
		/// <param name="storedProcedure">The stored procedure.</param>
		/// <param name="saveType">Type of the save.</param>
		/// <param name="userId">The user identifier.</param>
		/// <returns></returns>
		Task<int> Save<T>(object obj, string storedProcedure, int saveType, int userId);

		/// <summary>
		/// Deletes the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="storedProcedure">The stored procedure.</param>
		/// <returns></returns>
		Task<bool> Delete(int id, string storedProcedure);
	}
}
