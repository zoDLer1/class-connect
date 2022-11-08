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

    private async Task<List<Folder>> PrepareAllGroups()
    {
        var groups = await _groupStorageService.GetAllAsync();
        var result = new List<Folder>();
        foreach (var group in groups)
        {
            try
            {
                var folder = await _fileSystemService.GetFolderInfoAsync(group.Id);
                result.Add(folder);
            }
            catch
            {
                continue;
            }
        }
        return result;
    }

    [HttpGet("byId")]
    public async Task<IActionResult> GetAsync(string? id)
    {
        if (id == null)
            return new JsonResult(await PrepareAllGroups());

        var group = await _groupStorageService.GetAsync(id);
        if (group == null)
            return BadRequest();

        try
        {
            return new JsonResult(await _fileSystemService.GetFolderInfoAsync(group.Id));
        }
        catch (ItemTypeException)
        {
            return BadRequest();
        }
        catch (ItemNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("byName")]
    public async Task<IActionResult> GetByNameAsync(string? name)
    {
        if (name == null)
            return new JsonResult(await PrepareAllGroups());

        var group = await _groupStorageService.GetByGroupNameAsync(name);
        if (group == null)
            return BadRequest();

        return await GetAsync(group.Id);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(string? name)
    {
        if (name == null || _fileSystemService.RootId == null || await _groupStorageService.GetByGroupNameAsync(name) != null)
            return BadRequest();

        try
        {
            Item item = await _fileSystemService.CreateFolderAsync(_fileSystemService.RootId, name, 3);
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

    [HttpPatch("byId")]
    public async Task<IActionResult> RenameAsync(string? id, string? newName)
    {
        if (id == null || newName == null)
            return BadRequest();
        
        var groupInfo = await _groupStorageService.GetAsync(id);
        if (groupInfo == null)
            return BadRequest();

        try
        {
            await _fileSystemService.RenameAsync(groupInfo.Id, newName);
        }
        catch (ItemNotFoundException)
        {
            return NotFound();
        }
        catch (ItemTypeException)
        {
            return BadRequest();
        }
        finally
        {
            groupInfo.Name = newName;
            await _groupStorageService.UpdateAsync(groupInfo.Id, groupInfo);
        }

        return Ok();
    }

    [HttpPatch("byName")]
    public async Task<IActionResult> RenameByNameAsync(string? name, string? newName)
    {
        if (name == null || newName == null)
            return BadRequest();

        var group = await _groupStorageService.GetByGroupNameAsync(name);
        if (group == null)
            return BadRequest();

        return await RenameAsync(group.Id, newName);
    }

    [HttpDelete("byId")]
    public async Task<IActionResult> DeleteAsync(string? id)
    {
        if (id == null)
            return BadRequest();

        var groupInfo = await _groupStorageService.GetAsync(id);
        if (groupInfo == null)
            return BadRequest();

        try
        {
            await _fileSystemService.RemoveFolder(groupInfo.Id);
        }
        catch (ItemTypeException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            if (ex is FolderNotFoundException || ex is ItemNotFoundException)
                return NotFound();
            throw;
        }
        finally
        {
            await _groupStorageService.DeleteAsync(groupInfo.Id);
        }

        return Ok();
    }

    [HttpDelete("byId")]
    public async Task<IActionResult> DeleteByNameAsync(string? name)
    {
        if (name == null)
            return BadRequest();

        var group = await _groupStorageService.GetByGroupNameAsync(name);
        if (group == null)
            return BadRequest();

        return await DeleteAsync(group.Id);
    }
}