using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public class FolderHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, FileEntity> _commonFileQueries;
    
    public FolderHelperService(IHostEnvironment env, Context context) : base(env, context)
    {
        _commonFileQueries = new CommonQueries<string, FileEntity>(_context);
    }

    public async Task<(string, FolderItem)> CreateAsync(string parentId, string name, Dictionary<string, string>? parameters=null)
    {
        var result = await base.CreateAsync(parentId, name, 1);
        return result;
    }

    public async new Task DeleteAsync(string id)
    {
        var path = await base.DeleteAsync(id);
        Directory.Delete(path, true);
    }
}