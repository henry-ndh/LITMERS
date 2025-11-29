using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFU.Common.Models;


namespace TFU.Common.Extension
{
	public static class EFCoreExtension
	{
		public static IQueryable<T> ToPagedList<T>(this IQueryable<T> list, int pageNumber, int pageSize)
		{
			return list.Skip((pageNumber - 1) * pageSize).Take(pageSize);
		}

		public static IEnumerable<T> ToPagedList<T>(this IEnumerable<T> list, int pageNumber, int pageSize)
		{
			return list.Skip((pageNumber - 1) * pageSize).Take(pageSize);
		}


	}
}
