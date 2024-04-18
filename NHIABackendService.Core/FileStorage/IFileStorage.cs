using System.IO;
using Microsoft.Extensions.FileProviders;

namespace NHIABackendService.Core.FileStorage
{
    public interface IFileStorage : IFileInfo
    {
        string GetFileType();

        Stream OpenRead();

        Stream OpenWrite();

        Stream CreateFile();
    }
}
