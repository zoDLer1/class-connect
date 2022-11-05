using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Services.GroupStorageServices;
using PracticeWeb.Services.ItemStorageServices;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupController : ControllerBase
{
    private IGroupStorageService _groupStorageService;
    private IItemStorageService _itemStorageService;

        public GroupController(
        IGroupStorageService groupStorageService,
        IItemStorageService itemStorageService)
    {
        _groupStorageService = groupStorageService;
        _itemStorageService = itemStorageService;
    }

    private async Task<List<string>> MakeFullPathAsync(string childId)
    {
        var connection = await _itemStorageService.GetConnectionByChildAsync(childId);
        if (connection == null)
            return new List<string>() { childId };

        var result = await MakeFullPathAsync(connection.ParentId);
        result.Add(connection.ChildId);
        return result;
    }

    private async Task<List<FolderItem>> PrepareItemsAsync(List<string> itemIds)
    {
        var result = new List<FolderItem>();
        foreach (var id in itemIds)
        {
            var item = await _itemStorageService.GetAsync(id);
            if (item == null)
                continue;
            var path = string.Join(Path.DirectorySeparatorChar, await MakeFullPathAsync(item.Id));
            result.Add(new FolderItem() 
            {
                Name = item.Name,
                Path = item.Name,
                Guid = item.Id,
                Type = item.Type,
                MimeType = (item.Type.Name == "File" ? MimeTypes.GetMimeType(path) : null),
                CreationTime = item.CreationTime,
                CreatorName = "testName",
            });
        }
        return result;
    }

    [HttpGet()]
    public async Task<IActionResult> GetByGroup(string? name)
    {
        if (name == null)
            return BadRequest();

        var groupInfo = await _groupStorageService.GetByGroupNameAsync(name);
        if (groupInfo == null)
            return BadRequest();

        var item = await _itemStorageService.GetAsync(groupInfo.Id);
        if (item == null)
            return BadRequest();

        var items = await _itemStorageService.GetConnectionsByParentAsync(item.Id);
        var folder = new Folder 
        {
            Name = item.Name, 
            Path = item.Name, 
            Guid = item.Id,
            Items = await PrepareItemsAsync(items.Select(i => i.ChildId).ToList()),
            CreationTime = item.CreationTime,
            CreatorName = "testName",
        };
        return new JsonResult(folder);
    }
}