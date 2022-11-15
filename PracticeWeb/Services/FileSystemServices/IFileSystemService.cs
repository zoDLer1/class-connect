using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices;

public interface IFileSystemService
{
    public string? RootId { get; }
    Task<FileResult> GetFileAsync(string id);
    Task<Folder> GetFolderInfoAsync(string id);
    Task<FolderItem> CreateFileAsync(string parentId, IFormFile file);
    Task<FolderItem> CreateFolderAsync(string parentId, string name, string type, Dictionary<string, string>? parameters);
    Task RenameAsync(string id, string newName);
    Task UpdateTypeAsync(string id, string newType);
    Task RemoveAsync(string id);
}