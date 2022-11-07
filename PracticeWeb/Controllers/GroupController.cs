using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;
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
    public async Task<IActionResult> GetByGroupAsync(string? name)
    {
        if (name == null)
            return BadRequest();

        var groupInfo = await _groupStorageService.GetByGroupNameAsync(name);
        if (groupInfo == null)
            return BadRequest();

        return new JsonResult(await _fileSystemService.GetFolderInfoAsync(groupInfo.Id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(string? name)
    {
        if (name == null || _fileSystemService.RootId == null || await _groupStorageService.GetByGroupNameAsync(name) != null)
            return BadRequest();

        try
        {
            Item item = await _fileSystemService.CreateFolderAsync(_fileSystemService.RootId, name);
            var group = new Group
            {
                Id = item.Id,
                Name = name
            };
            await _groupStorageService.CreateAsync(group);
        }
        catch (Exception ex)
        {
            if (ex is FolderNotFoundException || ex is ItemNotFoundException)
                return NotFound();
            else if (ex is ItemTypeException)
                return BadRequest();
            throw;
        }

        return Ok();
    }
}