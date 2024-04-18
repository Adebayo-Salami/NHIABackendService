using Microsoft.AspNetCore.Http;

namespace NHIABackendService.Services.Interface
{
    public interface IBaseRequestService
    {
        string ConvertFileToBase64(string path);
        string ConvertFileToBase64(IFormFile file);
        void DeleteFiles(List<string> filePaths);
        List<string> TryUploadDocuments(List<IFormFile> formFiles);
    }
}
