using System.Collections.Generic;

namespace GameStudio.Repository
{
    public interface IGetPagedResults
    {
        int Page { get; set; }
        int Size { get; set; }
        int? Total { get; set; }
        int? Pages { get; }
        bool? HasMorePages { get; }
    }

    public interface IGetPagedResults<T> : IGetPagedResults
    {
        IReadOnlyList<T> Results { get; set; }
    }

    public class GetPagedResults<T> : IGetPagedResults<T>
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int? Total { get; set; }
        public int? Pages { get; }
        public bool? HasMorePages { get; }
        public IReadOnlyList<T> Results { get; set; }

        public static GetPagedResults<TResult> FromQueryResults<TResult>(IReadOnlyList<TResult> results, int page, int size, int? total = null)
        {
            return new GetPagedResults<TResult>
            {
                Page = page,
                Size = size,
                Total = total,
                Results = results
            };
        }
    }

    public class GetPagedOptions
    {
        public SortOptions Sort { get; set; }

        public static GetPagedOptions Default = new GetPagedOptions();

        public override string ToString()
        {
            return Sort == null ? string.Empty : Sort.ToString();
        }
    }

    public class SortOptions
    {
        public string Field { get; set; }
        public bool? Ascending { get; set; }
        public bool? Descending { get; set; }

        public override string ToString()
        {
            string sort = "";
            if (Ascending.GetValueOrDefault(false))
                sort = "asc";
            if (Descending.GetValueOrDefault(false))
                sort = "desc";

            return $"sort={sort}&sortField={Field}";
        }
    }

    public class PagedQuery
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public GetPagedOptions Options { get; set; }

        public static PagedQuery Create(int page = 1, int size = 50, GetPagedOptions options = null)
        {
            return new PagedQuery
            {
                Page = page,
                Size = size,
                Options = options ?? GetPagedOptions.Default
            };
        }

        public override string ToString()
        {
            return $"page={Page}&size={Size}&options={Options}";
        }
    }
}
