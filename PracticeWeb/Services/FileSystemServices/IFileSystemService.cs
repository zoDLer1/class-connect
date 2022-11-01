using Microsoft.AspNetCore.Mvc;

namespace PracticeWeb.Services.FileSystemServices;

public interface IFileSystemService
{
    Task<FileResult> GetFile(string path);
    Task<Folder> GetFolderInfo(string path);
    Task CreateFile(string path, IFormFile file);
    Task CreateFolder(string path);
    Task Rename(string path, string newName);
    Task Remove(string path);
}