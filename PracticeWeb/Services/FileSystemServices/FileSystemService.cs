using Microsoft.AspNetCore.Mvc;

namespace PracticeWeb.Services.FileSystemServices;

public class FileSystemService : IFileSystemService
{
    public async Task<FileResult> GetFile(string path)
    {
        throw new NotImplementedException();
    }

    public async Task<Folder> GetFolderInfo(string path)
    {
        throw new NotImplementedException();
    }

    public async void CreateFile(string path, IFormFile file)
    {
        throw new NotImplementedException();
    }

    public async void CreateFolder(string path)
    {
        throw new NotImplementedException();
    }

    public async void Rename(string path, string newName)
    {
        throw new NotImplementedException();
    }

    public async void Remove(string path)
    {
        throw new NotImplementedException();
    }
}