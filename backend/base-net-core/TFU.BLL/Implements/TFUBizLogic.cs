using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TFU.BLL.Interfaces;
using TFU.Common.Models;
using TFU.DAL.Interfaces;

namespace TFU.BLL.Implements
{
	/// <summary>
	/// Customer repository
	/// </summary>
	/// <seealso cref="Techover.Repository.Interfaces.ITFURepository" />
	public class TFUBizLogic : ITFUBizLogic
	{
		ITFURepository _tfuRepository;
		public TFUBizLogic(ITFURepository tfuRepository)
		{
			_tfuRepository = tfuRepository;
		}

		public async Task<IEnumerable<T>> GetAll<T>(string storedProcedure)
		{
			return await _tfuRepository.GetAll<T>(storedProcedure);
		}

		public async Task<IEnumerable<T>> GetListPaging<T>(PagingModel pagingModel, string storedProcedure)
		{
			return await _tfuRepository.GetListPaging<T>(pagingModel, storedProcedure);
		}

		public async Task<List<T1>> GetListPaging<T1, T2>(PagingModel pagingModel, string storedProcedure, List<KeyValuePair<string, string>> sqlParams = null)
		{
			return await _tfuRepository.GetListPaging<T1, T2>(pagingModel, storedProcedure, sqlParams);
		}

		public Task<bool> Delete(int id, string storedProcedure)
		{
			throw new NotImplementedException();
		}

		public Task<T> GetByID<T>(int id, string storedProcedure)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Dropdown>> GetDropdown(string storedProcedure)
		{
			throw new NotImplementedException();
		}

		public Task<int> Save<T>(object obj, string storedProcedure, int saveType, int userId)
		{
			throw new NotImplementedException();
		}
	}
}
