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
                var sortedList = ApplySorting(inputList, orderByProperty, orderBy);
                if (sortedList != null)
                {
                    inputList = sortedList;
                }
            }


            response.List = inputList;


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

        private IEnumerable<T> ApplySorting(IEnumerable<T> inputList, string orderByProperty, string? orderBy)
        {
            var properties = orderByProperty.Split('.');
            var param = Expression.Parameter(typeof(T), "x");
            Expression propertyAccess = param;
            // Creating a null check expression
            Expression nullCheckExpression = null;

            foreach (var property in properties)
            {
                var propertyInfo = typeof(T).GetProperty(property);
                if (propertyInfo != null)
                {
                    var propertyAccessTemp = Expression.Property(propertyAccess, property);
                    // Check if property type is a value type and convert to nullable if necessary

                    // Check if property type is a value type and convert to nullable if necessary
                    Expression nullCheckTemp;
                    if (propertyAccessTemp.Type.IsValueType && Nullable.GetUnderlyingType(propertyAccessTemp.Type) == null)
                    {
                        var nullableType = typeof(Nullable<>).MakeGenericType(propertyAccessTemp.Type);
                        var converted1 = Expression.Convert(propertyAccessTemp, nullableType);
                        nullCheckTemp = Expression.NotEqual(converted1, Expression.Constant(null, nullableType));
                    }
                    else
                    {
                        nullCheckTemp = Expression.NotEqual(propertyAccessTemp, Expression.Constant(null, propertyAccessTemp.Type));
                    }

                    // Combining null checks for nested properties
                    nullCheckExpression = nullCheckExpression == null
                        ? nullCheckTemp
                        : Expression.AndAlso(nullCheckExpression, nullCheckTemp);

                    propertyAccess = propertyAccessTemp;
                }
            }

            // Handling null values by converting the property access to a nullable type
            var converted = Expression.Convert(propertyAccess, typeof(object));

            var orderByExpression = Expression.Lambda<Func<T, object>>(converted, param);

            // Applying null check filter before sorting
            if (nullCheckExpression != null)
            {
                var nullCheckLambda = Expression.Lambda<Func<T, bool>>(nullCheckExpression, param);
                inputList = inputList.AsQueryable().Where(nullCheckLambda);
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                if (orderBy.ToLower() == "asc")
                {
                    return inputList.AsQueryable().OrderBy(orderByExpression).ToList();
                }
                else if (orderBy.ToLower() == "desc")
                {
                    return inputList.AsQueryable().OrderByDescending(orderByExpression).ToList();
                }
            }

            return inputList;
        }
    }
}
