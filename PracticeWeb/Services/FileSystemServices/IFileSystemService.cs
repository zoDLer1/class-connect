using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices;

public interface IFileSystemService
{
    public string? RootGuid { get; }
    Task RecreateFileSystemAsync();
    Task<Object> SubmitWork(string id, User user);
    Task<Object> MarkWork(string id, int mark, User user);
    Task<Object> GetObjectAsync(string id, User user);
    Task<Object> CreateWorkAsync(string parentId, string name, string type, IFormFile file, User user);
    Task<Object> CreateFileAsync(string parentId, IFormFile file, User user);
    Task<Object> CreateFolderAsync(string parentId, string name, string type, User user, Dictionary<string, string>? parameters);
    Task RenameAsync(string id, string newName, User user);
    Task UpdateTypeAsync(string id, string newType, User user);
    Task RemoveAsync(string id, User user);
}
