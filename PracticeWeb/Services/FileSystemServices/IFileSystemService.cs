using Microsoft.AspNetCore.Mvc;

namespace PracticeWeb.Services.FileSystemServices;

public interface IFileSystemService
{
    Task<FileResult> GetFile(string path);
    Task<Folder> GetFolderInfo(string path);
    void CreateFile(string path, IFormFile file);
    void CreateFolder(string path);
    void Rename(string path, string newName);
    void Remove(string path);
}