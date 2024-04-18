using NHIABackendService.Core.DataAccess.Repository;
using NHIABackendService.Core.ViewModels.Enums;
using NHIABackendService.Core.ViewModels;
using NHIABackendService.Entities;
using NHIABackendService.Services.Interface;
using NHIABackendService.Services.Model;
using System.ComponentModel.DataAnnotations;
using NHIABackendService.Services.Dto;

#nullable disable

namespace NHIABackendService.Services.Services
{
    public class FileService : IFileService
    {
        private readonly IBaseRequestService _baseRequestService;
        private readonly IRepository<FileUpload, Guid> _fileUploadRepository;

        public FileService(IBaseRequestService baseRequestService, IRepository<FileUpload, Guid> fileUploadRepository)
        {
            _baseRequestService = baseRequestService;
            _fileUploadRepository = fileUploadRepository;
        }

        public ResultModel<UploadedFileVm> DownloadTemplate(FileTemplateType templateType)
        {
            string fileName;

            switch (templateType)
            {
                case FileTemplateType.DEFAULT:
                    fileName = "Default_Template.xlsx";
                    break;

                default:
                    return new ResultModel<UploadedFileVm>
                    {
                        Data = null,
                        Message = "Unable to find the specified template type.",
                        ValidationErrors = new List<ValidationResult>
                            {new ValidationResult("Unable to find the specified template type.", null)}
                    };
            }

            var file = new UploadedFileVm
            {
                Document = _baseRequestService.ConvertFileToBase64(fileName),
                Name = fileName.Replace('_', ' ')
            };

            return new ResultModel<UploadedFileVm> { Data = file, Message = "Success" };
        }

        public List<FileDto> GetFiles(List<Guid> ids)
        {
            return _fileUploadRepository.GetAll().Where(x => ids.Contains(x.Id)).Select(x => new FileDto
            {
                Id = x.Id,
                ContentType = x.ContentType,
                Name = x.Name,
                Path = x.Path
            }).ToList();
        }
    }
}
