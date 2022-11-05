using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Services.FileSystemServices;
using PracticeWeb.Services.GroupStorageServices;
using PracticeWeb.Services.ItemStorageServices;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupController : ControllerBase
{
    private IFileSystemService _fileSystemService;
    private IGroupStorageService _groupStorageService;
    private IItemStorageService _itemStorageService;

        public GroupController(
        IFileSystemService fileSystemService, 
        IGroupStorageService groupStorageService,
        IItemStorageService itemStorageService)
    {
        _fileSystemService = fileSystemService;
        _groupStorageService = groupStorageService;
        _itemStorageService = itemStorageService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetByGroup(string? name)
    {
        if (name == null)
            return BadRequest();

        var groupInfo = await _groupStorageService.GetByGroupNameAsync(name);
        if (groupInfo == null)
            return BadRequest();

        return new JsonResult(await _fileSystemService.GetFolderInfoAsync(groupInfo.Id));
    }
}