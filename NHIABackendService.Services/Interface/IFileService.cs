using NHIABackendService.Core.ViewModels;
using NHIABackendService.Core.ViewModels.Enums;
using NHIABackendService.Services.Dto;
using NHIABackendService.Services.Model;

namespace NHIABackendService.Services.Interface
{
    public interface IFileService
    {
        ResultModel<UploadedFileVm> DownloadTemplate(FileTemplateType templateType);

        List<FileDto> GetFiles(List<Guid> ids);
    }
}
