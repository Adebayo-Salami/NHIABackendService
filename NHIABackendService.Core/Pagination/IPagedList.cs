using System.Collections.Generic;

namespace NHIABackendService.Core.Pagination
{
    public interface IPagedList<out T> : IPagedList, IEnumerable<T>
    {
        T this[int index] { get; }

        int Count { get; }

        IEnumerable<T> Items { get; }

        IPagedList GetMetaData();
    }

    public interface IPagedList
    {
        int PageCount { get; }

        int TotalItemCount { get; }

        int PageNumber { get; }

        int PageSize { get; }

        bool HasPreviousPage { get; }

        bool HasNextPage { get; }
    }
}
