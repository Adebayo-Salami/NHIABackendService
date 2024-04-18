#nullable disable

namespace NHIABackendService.Core.ViewModels
{
    public class UploadedFileVm
    {
        public string Name { get; set; }

        public Guid FileUploadId { get; set; }

        public string ContentType { get; set; }

        public string Document { get; set; }
    }

    public class UploadedSignature : UploadedFileVm
    {
        public int Id { get; set; }
    }
}
