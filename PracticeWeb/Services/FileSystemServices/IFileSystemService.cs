using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices;

public interface IFileSystemService
{
    public string? RootId { get; }
    Task<FileResult> GetFileAsync(string id);
    Task<Folder> GetFolderInfoAsync(string id);
    Task<FolderItem> CreateFileAsync(string parentId, IFormFile file);
    Task<FolderItem> CreateFolderAsync(string parentId, string name);
    Task<FolderItem> CreateFolderAsync(string parentId, string name, int typeId);
    Task RenameAsync(string id, string newName);
    Task RemoveFileAsync(string id);
    Task RemoveFolder(string id);
}