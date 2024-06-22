using SU24_VMO_API.DTOs.Response;

namespace SU24_VMO_API.Services
{
    public class PaginationService<T>
    {
        public PaginationResponse<T> PaginateList(IEnumerable<T> inputList, int? pageSize, int? pageNo)
        {
            PaginationResponse<T> response = new PaginationResponse<T>
            {
                PageSize = pageSize ?? null,
                PageNo = pageNo ?? null,
                TotalItem = inputList.Count(),
                TotalPage = 1,
                List = inputList
            };

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
