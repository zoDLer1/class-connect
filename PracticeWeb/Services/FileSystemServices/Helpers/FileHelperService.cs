using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public class FileHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, FileEntity> _commonFileQueries;
    
    public FileHelperService(IHostEnvironment env, Context context) : base(env, context)
    {
        _commonFileQueries = new CommonQueries<string, FileEntity>(_context);
    }

    public async Task<(string, FolderItem)> CreateAsync(string parentId, string name, Dictionary<string, string>? parameters=null)
    {
        var (path, item) = await base.CreateAsync(parentId, name, 2);
        var fileEntity = new FileEntity
        {
            Id = item.Guid,
            Extension = Path.GetExtension(item.Name),
            MimeType = MimeTypes.GetMimeType(item.Name)
        };
        await _commonFileQueries.CreateAsync(fileEntity);
        return (path, item);
    }

    public async new Task DeleteAsync(string id)
    {
        await _commonFileQueries.DeleteAsync(id);
        var path = await base.DeleteAsync(id);
        File.Delete(path);
    }
}