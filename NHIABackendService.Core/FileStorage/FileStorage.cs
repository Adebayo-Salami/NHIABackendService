using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NHIABackendService.Core.FileStorage
{
    [ExcludeFromCodeCoverage]
    public class FileStorage
    {
        private readonly FileInfo _fileInfo;

        public FileStorage(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public string GetFileType()
        {
            return _fileInfo.Extension;
        }

        public Stream OpenRead()
        {
            return new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read);
        }

        public Stream OpenWrite()
        {
            return new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
        }

        public Stream CreateFile()
        {
            return new FileStream(_fileInfo.FullName, FileMode.Truncate, FileAccess.ReadWrite);
        }
    }
}
