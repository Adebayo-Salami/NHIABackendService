using NHIABackendService.Core.Utility;
using NHIABackendService.Core.ViewModels.Enums;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace NHIABackendService.Core.ViewModels
{
    public class SearchVM
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageIndex must be greater than 0")]
        public int PageIndex { get; set; } = CoreConstants.PaginationConsts.PageIndex;

        public int? PageTotal { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be greater than 0")]
        public int PageSize { get; set; } = 10;

        public int PageSkip => (PageIndex - 1) * PageSize;

        public List<Filter> Filters { get; set; } = new List<Filter>();
    }

    public class Filter
    {
        [Required] public string Keyword { get; set; }

        [Required] public string FilterColumn { get; set; }

        [Required] public FilterOperation? Operation { get; set; }
    }
}
