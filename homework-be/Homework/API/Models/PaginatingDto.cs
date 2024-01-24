namespace Homework.Gateway.API.Models
{
    public class PaginatingDto<T> where T : class
    {
        public T[] Data { get; set; }
        public int TotalRecords { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
