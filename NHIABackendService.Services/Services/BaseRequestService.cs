using Microsoft.AspNetCore.Http;
using NHIABackendService.Core.FileStorage;
using NHIABackendService.Core.Utility;
using NHIABackendService.Services.Interface;

namespace NHIABackendService.Services.Services
{
    public class BaseRequestService : IBaseRequestService
    {
        private readonly IFileStorageService _fileStorageService;

        public BaseRequestService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public string ConvertFileToBase64(string path)
        {
            var file = _fileStorageService.GetFile(path);
            string base64String;
            using (var fileStream = file.CreateReadStream())
            {
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    base64String = Convert.ToBase64String(fileBytes);
                }
            }

            return base64String;
        }

        public string ConvertFileToBase64(IFormFile file)
        {
            string base64String;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                base64String = Convert.ToBase64String(fileBytes);
            }

            return base64String;
        }

        public void DeleteFiles(List<string> filePaths)
        {
            foreach (var filePath in filePaths) _fileStorageService.DeleteFile(filePath);
        }

        public List<string> TryUploadDocuments(List<IFormFile> formFiles)
        {
            var filePaths = new List<string>();
            var uploaded = false;
            foreach (var file in formFiles)
            {
                var fileName = CommonHelper.GenerateTimeStampedFileName(file.FileName);
                uploaded = _fileStorageService.TrySaveStream(fileName,
                    file.OpenReadStream());
                if (uploaded)
                    filePaths.Add(fileName);
                else
                    break;
            }

            if (!uploaded && filePaths.Count > 0)
            {
                foreach (var file in filePaths)
                    _fileStorageService.DeleteFile(file);

                filePaths.Clear();
            }

            return filePaths;
        }
    }
}
