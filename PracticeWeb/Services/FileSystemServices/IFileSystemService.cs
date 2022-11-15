using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices;

public interface IFileSystemService
{
    public string? RootGuid { get; }
    Task RecreateFileSystemAsync();
    Task<FileResult> GetFileAsync(string id);
    Task<Folder> GetFolderInfoAsync(string id);
    Task<FolderItem> CreateFileAsync(string parentId, IFormFile file, int creatorId);
    Task<FolderItem> CreateFolderAsync(string parentId, string name, string type, int creatorId, Dictionary<string, string>? parameters);
    Task RenameAsync(string id, string newName);
    Task UpdateTypeAsync(string id, string newType);
    Task RemoveAsync(string id);
}