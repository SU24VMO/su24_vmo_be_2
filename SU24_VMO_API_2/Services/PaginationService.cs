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
                var propertyInfo = typeof(T).GetProperty(orderByProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    if (!string.IsNullOrEmpty(orderBy))
                    {
                        if (orderBy.ToLower() == "asc")
                        {
                            inputList = inputList.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
                        }
                        else if (orderBy.ToLower() == "desc")
                        {
                            inputList = inputList.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
                        }
                    }
                }
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

            return response;
        }
    }
}
