namespace SU24_VMO_API.DTOs.Response
{
    public class PaginationResponse<T>
    {
        public int TotalItem { get; set; }

        public int TotalPage { get; set; }

        public int? PageSize { get; set; }

        public int? PageNo { get; set; }

        public IEnumerable<T> List { get; set; } = new List<T>();
    }
}
