using SU24_VMO_API.DTOs.Response;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SU24_VMO_API.Services
{
    public class PaginationService<T>
    {
        public PaginationResponse<T> PaginateList(IEnumerable<T> inputList, int? pageSize, int? pageNo, string? orderBy, string? orderByProperty = null)
        {
            PaginationResponse<T> response = new PaginationResponse<T>
            {
                PageSize = pageSize ?? null,
                PageNo = pageNo ?? null,
                TotalItem = inputList.Count(),
                TotalPage = 1,
                List = inputList
            };




            // Apply sorting based on the orderBy parameter and selector
            if (!string.IsNullOrEmpty(orderByProperty))
            {
                // Split the orderByProperty by dot to support nested properties
                var properties = orderByProperty.Split('.');
                var param = Expression.Parameter(typeof(T));
                Expression propertyAccess = param;

                foreach (var property in properties)
                {
                    propertyAccess = Expression.Property(propertyAccess, property);
                }

                var orderByExpression = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyAccess, typeof(object)), param);


                //var propertyInfo = typeof(T).GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                //if (propertyInfo != null)
                //{
                if (!string.IsNullOrEmpty(orderBy))
                {
                    if (orderBy.ToLower() == "asc")
                    {
                        inputList = inputList.AsQueryable().OrderBy(orderByExpression).ToList();
                    }
                    else if (orderBy.ToLower() == "desc")
                    {
                        inputList = inputList.AsQueryable().OrderByDescending(orderByExpression).ToList();
                    }
                }
                //}
            }


            if (pageSize.HasValue && pageNo.HasValue)
            {

                if (pageSize <= 0 || pageNo <= 0)
                {
                    response.List = Enumerable.Empty<T>();
                }

                int startIndex = (pageNo.Value - 1) * pageSize.Value;

                if (startIndex < inputList.Count())
                {
                    response.List = inputList.Skip(startIndex).Take(pageSize.Value);
                }
                else
                {
                    response.List = Enumerable.Empty<T>();
                }

                response.TotalPage = (int)Math.Ceiling((double)response.TotalItem / pageSize.Value);
            }

            response.List = inputList;

            return response;
        }
    }
}
