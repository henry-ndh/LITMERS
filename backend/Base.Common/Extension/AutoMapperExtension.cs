using App.Entity.Models.Wapper;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TFU.Common.Attributes;

namespace TFU.Common.Extension
{
  public static class AutoMapperExtension
  {
    public static PagedResult<TDestination> MapPagedResult<TSource, TDestination>(this PagedResult<TSource> source, IMapper mapper)
        {
            var mappedList = mapper.Map<List<TDestination>>(source.ListObjects);

            return new PagedResult<TDestination>(
                mappedList,
                source.TotalRecords,
                source.PageNumber,
                source.PageSize
            );
        }     
    }
}
