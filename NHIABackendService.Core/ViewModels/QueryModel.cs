using static NHIABackendService.Core.Utility.CoreConstants;

#nullable disable

namespace NHIABackendService.Core.ViewModels
{
    public class QueryModel : PagedRequestModel
    {
        public string Keyword { get; set; }

        public string Filter { get; set; }
    }

    public class PagedRequestModel
    {
        public int PageIndex { get; set; } = PaginationConsts.PageIndex;

        public int PageSize { get; set; } = PaginationConsts.PageSize;
    }
}
