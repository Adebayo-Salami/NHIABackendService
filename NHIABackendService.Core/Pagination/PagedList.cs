using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NHIABackendService.Core.Pagination
{
    [JsonObject]
    public class PagedList<T> : BasePagedList<T>
    {
        public PagedList(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)
        {
            TotalItemCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            Subset.AddRange(items);
        }

        public PagedList(IQueryable<T> superset, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "PageNumber cannot be below 1.");
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "PageSize cannot be less than 1.");

            // set source to blank list if superset is null to prevent exceptions
            TotalItemCount = superset == null ? 0 : superset.Count();
            PageSize = pageSize;
            PageNumber = pageNumber;
            PageCount = TotalItemCount > 0
                ? (int)Math.Ceiling(TotalItemCount / (double)PageSize)
                : 0;

            // add items to internal list
            if (superset != null && TotalItemCount > 0)
                Subset.AddRange(pageNumber == 1
                    ? superset.Skip(0).Take(pageSize).ToList()
                    : superset.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                );
        }

        public PagedList(IEnumerable<T> superset, int pageNumber, int pageSize) : this(superset.AsQueryable(), pageNumber, pageSize)
        {
        }

        public override IEnumerable<T> Items => Subset;
    }
}
