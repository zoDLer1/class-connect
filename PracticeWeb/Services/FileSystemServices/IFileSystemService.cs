using Microsoft.AspNetCore.Mvc;

namespace PracticeWeb.Services.FileSystemServices;

public interface IFileSystemService
{
    Task<FileResult> GetFileAsync(string id);
    Task<Folder> GetFolderInfoAsync(string id);
    Task CreateFileAsync(string parentId, IFormFile file);
    Task CreateFolderAsync(string parentId, string name);
    Task RenameAsync(string id, string newName);
    Task RemoveFileAsync(string id);
    Task RemoveFolder(string id);
}