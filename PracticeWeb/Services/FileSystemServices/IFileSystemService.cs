using Microsoft.AspNetCore.Mvc;

namespace PracticeWeb.Services.FileSystemServices;

public interface IFileSystemService
{
    Task<FileResult> GetFileAsync(string path);
    Task<Folder> GetFolderInfoAsync(string path);
    Task CreateFileAsync(string path, IFormFile file);
    Task CreateFolderAsync(string path, string name);
    Task RenameAsync(string path, string newName);
    Task RemoveFileAsync(string path);
    Task RemoveFolder(string path);
}