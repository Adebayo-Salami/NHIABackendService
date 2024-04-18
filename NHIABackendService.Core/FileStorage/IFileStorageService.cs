using Microsoft.Extensions.FileProviders;

namespace NHIABackendService.Core.FileStorage
{
    public interface IFileStorageService
    {
        bool FileExists(string path);

        bool FolderExists(string path);

        bool TryCreateFolder(string path);

        void CreateFolder(string path);

        void DeleteFile(string path);

        void RenameFile(string oldPath, string newPath);

        FileInfo CreateFile(string path);

        IFileInfo GetFile(string path);

        bool TrySaveStream(string path, Stream inputStream);

        void SaveStream(string path, Stream inputStream);

        void SaveBytes(string path, byte[] raw);

        void WriteAllText(string path, string content);

        string ReadAllText(string path);

        bool TrySaveBytes(string path, byte[] raw);

        string RenameDuplicateFile(string filepath);

        bool ValidateFile(string[] allowedExt, string fileName);
    }
}
