using System;

namespace NHIABackendService.Core.Pagination
{
    [Serializable]
    public class PagedListMetaData : IPagedList
    {
        protected PagedListMetaData()
        {
        }

        public PagedListMetaData(IPagedList pagedList)
        {
            PageCount = pagedList.PageCount;
            TotalItemCount = pagedList.TotalItemCount;
            PageNumber = pagedList.PageNumber;
            PageSize = pagedList.PageSize;
        }

        public int PageCount { get; protected set; }

        public int TotalItemCount { get; protected set; }

        public int PageNumber { get; protected set; }

        public int PageSize { get; protected set; }

        public bool HasPreviousPage { get; protected set; }

        public bool HasNextPage { get; protected set; }
    }
}
