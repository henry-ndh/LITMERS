using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TFU.Common;
using TFU.Common.Models;
using TFU.DAL.Interfaces;

namespace TFU.DAL.Implements
{
    /// <summary>
    /// Customer repository
    /// </summary>
    /// <seealso cref="Techover.Repository.Interfaces.ITFURepository" />
    public class TFURepository : BaseRepository, ITFURepository
    {
        public TFURepository(EntityFramework.TFUDbContext context, IConfiguration configuration) : base(configuration, context)
        {
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcedure">The stored procedure.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAll<T>(string storedProcedure)
        {
            List<T> list = new List<T>();
            var result = await Connection.QueryAsync<T>(storedProcedure, commandType: System.Data.CommandType.StoredProcedure);
            return result;
        }

        /// <summary>
        /// Gets the list paging.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagingModel">The paging model.</param>
        /// <param name="storedProcedure">The stored procedure.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetListPaging<T>(PagingModel pagingModel, string storedProcedure)
        {
            try
            {
                List<T> list = new List<T>();
                var param = AddDynamicParams(pagingModel);
                var result = await Connection.QueryAsync<T>(storedProcedure, param, commandType: CommandType.StoredProcedure);
                var total = param.Get<int>("@TotalRecord");
                pagingModel.TotalRecord = total;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

      /// <summary>
      /// Gets the list paging.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="pagingModel">The paging model.</param>
      /// <param name="storedProcedure">The stored procedure.</param>
      /// <returns></returns> 
      public async Task<List<T1>> GetListPaging<T1, T2>(PagingModel pagingModel, string storedProcedure, List<KeyValuePair<string, string>> sqlParams = null)
      {
         var xml = Utils.SerializeXML(pagingModel);
         try
         {
            List<T1> datas = null; 
            var param = new DynamicParameters();
            param.Add("@XML", xml, DbType.String, ParameterDirection.Input);
            if(sqlParams != null && sqlParams.Count > 0)
            {
               foreach(var p in sqlParams)
                  param.Add($"{p.Key}", p.Value, DbType.String, ParameterDirection.Input);
            }
            param.Add("@TotalRecord", 0, DbType.Int32, ParameterDirection.Output);
            var result = await Connection.QueryAsync<T1>(storedProcedure, param, commandType: CommandType.StoredProcedure);
            if (result != null) datas = result.ToList();
            var total = param.Get<int>("@TotalRecord");
            pagingModel.TotalRecord = total;
            return datas;
         }
         catch (Exception ex)
         {
            throw ex;
         }
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
