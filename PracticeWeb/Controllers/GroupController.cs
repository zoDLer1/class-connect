using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;
using PracticeWeb.Services.FileSystemServices;
using PracticeWeb.Services.GroupStorageServices;
using PracticeWeb.Services.SubjectStorageServices;
using PracticeWeb.Services.ItemStorageServices;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupController : ControllerBase
{
    private IFileSystemService _fileSystemService;
    private IGroupStorageService _groupStorageService;
    private ISubjectStorageService _subjectStorageService;
    private IItemStorageService _itemStorageService;

        public GroupController(
        IFileSystemService fileSystemService, 
        IGroupStorageService groupStorageService,
        ISubjectStorageService subjectStorageService,
        IItemStorageService itemStorageService)
    {
        _fileSystemService = fileSystemService;
        _groupStorageService = groupStorageService;
        _subjectStorageService = subjectStorageService;
        _itemStorageService = itemStorageService;
    }

    private async Task<List<Folder>> PrepareAllGroupsAsync()
    {
        var groups = await _groupStorageService.GetAllAsync();
        var result = new List<Folder>();
        foreach (var group in groups.OrderBy(g => g.Name))
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

    private async Task<List<SubjectFolder>> PrepareAllSubjectsAsync(string groupId)
    {
        var subjects = await _subjectStorageService.GetByGroupAsync(groupId);
        var result = new List<SubjectFolder>();
        foreach (var subject in subjects)
        {
            try
            {
                var folder = await _fileSystemService.GetFolderInfoAsync(subject.Id);
                var subjectFolder = new SubjectFolder
                {
                    Name = folder.Name, 
                    Type = folder.Type,
                    Path = folder.Path,
                    RealPath = folder.RealPath,
                    Guid = folder.Guid,
                    Items = folder.Items,
                    CreationTime = folder.CreationTime,
                    CreatorName = folder.CreatorName,
                    Description = subject.Description
                };
                result.Add(subjectFolder);
            }
            catch
            {
                continue;
            }
        }
        return result;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        return new JsonResult(await PrepareAllGroupsAsync());
    }

    [HttpGet("byId")]
    public async Task<IActionResult> GetByIdAsync(string? id)
    {
        if (id == null)
            return BadRequest();

        var group = await _groupStorageService.GetAsync(id);
        if (group == null)
            return NotFound();

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
            return BadRequest();

        var group = await _groupStorageService.GetByGroupNameAsync(name);
        if (group == null)
            return NotFound();

        return await GetByIdAsync(group.Id);
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
            return NotFound();

        try
        {
            await _fileSystemService.RenameAsync(groupInfo.Id, newName);
        }
        catch (ItemNotFoundException)
        {
            return NotFound();
        }

        groupInfo.Name = newName;
        await _groupStorageService.UpdateAsync(groupInfo);
        return Ok();
    }

    [HttpPatch("byName")]
    public async Task<IActionResult> RenameByNameAsync(string? name, string? newName)
    {
        if (name == null || newName == null)
            return BadRequest();

        var group = await _groupStorageService.GetByGroupNameAsync(name);
        if (group == null)
            return NotFound();

        return await RenameAsync(group.Id, newName);
    }

    [HttpDelete("byId")]
    public async Task<IActionResult> DeleteAsync(string? id)
    {
        if (id == null)
            return BadRequest();

        var groupInfo = await _groupStorageService.GetAsync(id);
        if (groupInfo == null)
            return NotFound();

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
        await _groupStorageService.DeleteAsync(groupInfo.Id);

        return Ok();
    }

    [HttpDelete("byName")]
    public async Task<IActionResult> DeleteByNameAsync(string? name)
    {
        if (name == null)
            return BadRequest();

        var group = await _groupStorageService.GetByGroupNameAsync(name);
        if (group == null)
            return NotFound();

        return await DeleteAsync(group.Id);
    }
}