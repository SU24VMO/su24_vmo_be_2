using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder.Capabilities.V1;
using SU24_VMO_API.DTOs.Response;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

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

                if (sortedList != null && sortedList.Any())
                {
                    //if (inputList.Count() == sortedList.Count())
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
                var propertyInfo = propertyAccess.Type.GetProperty(property);
                if (propertyInfo != null)
                {
                    var propertyAccessTemp = Expression.Property(propertyAccess, property);

                    // Check if property type is a value type and convert to nullable if necessary
                    Expression nullCheckTemp;
                    if (propertyAccessTemp.Type.IsValueType && Nullable.GetUnderlyingType(propertyAccessTemp.Type) == null)
                    {
                        var nullableType = typeof(Nullable<>).MakeGenericType(propertyAccessTemp.Type);
                        var converted = Expression.Convert(propertyAccessTemp, nullableType);
                        nullCheckTemp = Expression.NotEqual(converted, Expression.Constant(null, nullableType));
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
            var convertedPropertyAccess = Expression.Convert(propertyAccess, typeof(object));

            var orderByExpression = Expression.Lambda<Func<T, object>>(convertedPropertyAccess, param);

            // Applying null check filter before sorting
            if (nullCheckExpression != null)
            {
                var nullCheckLambda = Expression.Lambda<Func<T, bool>>(nullCheckExpression, param);
                inputList = inputList.AsQueryable().Where(nullCheckLambda);
            }

            // Determine the default orderBy direction (asc if not specified)

            bool isDescending = orderBy != null && orderBy.ToLower() == "desc";
            // Use custom comparer to handle null values during sorting

            var sortedList = isDescending
                    ? inputList.OrderByDescending(orderByExpression.Compile(), new CustomComparer())
                    : inputList.OrderBy(orderByExpression.Compile(), new CustomComparer());
            return sortedList.ToList();
        }

        private class CustomComparer : IComparer<object>
        {
            public int Compare(object x, object y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return 1;
                if (y == null) return -1;
                return Comparer<object>.Default.Compare(x, y);

            }
        }
    }
}
