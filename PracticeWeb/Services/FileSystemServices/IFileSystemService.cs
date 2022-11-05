using Microsoft.AspNetCore.Mvc;

namespace PracticeWeb.Services.FileSystemServices;

public interface IFileSystemService
{
    Task<FileResult> GetFileAsync(string path);
    Task<Folder> GetFolderInfoAsync(string id);
    Task CreateFileAsync(string path, IFormFile file);
    Task CreateFolderAsync(string parentId, string name);
    Task RenameAsync(string id, string newName);
    Task RemoveFileAsync(string path);
    Task RemoveFolder(string path);
}