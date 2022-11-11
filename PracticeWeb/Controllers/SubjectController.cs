using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;
using PracticeWeb.Services.FileSystemServices;
using PracticeWeb.Services.SubjectStorageServices;
using PracticeWeb.Services.GroupStorageServices;
using PracticeWeb.Services.ItemStorageServices;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectController : ControllerBase
{
    private IFileSystemService _fileSystemService;
    private ISubjectStorageService _subjectStorageService;
    private IGroupStorageService _groupStorageService;
    private IItemStorageService _itemStorageService;

    public SubjectController(
        IFileSystemService fileSystemService, 
        ISubjectStorageService subjectStorageService,
        IGroupStorageService groupStorageService,
        IItemStorageService itemStorageService)
    {
        _fileSystemService = fileSystemService;
        _subjectStorageService = subjectStorageService;
        _groupStorageService = groupStorageService;
        _itemStorageService = itemStorageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(string? id)
    {
        if (id == null)
            return BadRequest();

        var subject = await _subjectStorageService.GetAsync(id);
        if (subject == null)
            return NotFound();

        try
        {
            var folder = await _fileSystemService.GetFolderInfoAsync(id);
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
            return new JsonResult(subjectFolder);
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

    [HttpPost]
    public async Task<IActionResult> CreateAsync(string? name, string? groupId, string? description)
    {
        if (name == null || groupId == null)
            return BadRequest();

        var group = await _groupStorageService.GetAsync(groupId);
        var groupItem = await _itemStorageService.GetAsync(groupId);
        if (group == null || groupItem == null || groupItem.Type.Name != "Group")
            return NotFound();

        var anotherSubject = await _subjectStorageService.GetByGroupAndNameAsync(groupId, name);
        if (anotherSubject != null)
            return BadRequest();

        try
        {
            var item = await _fileSystemService.CreateFolderAsync(group.Id, name, 4);
            var subject = new Subject
            {
                Id = item.Guid,
                GroupId = group.Id,
                Name = name,
                Description = description
            };
            await _subjectStorageService.CreateAsync(subject);
            return new JsonResult(item);
        }
        catch (Exception ex)
        {
            if (ex is FolderNotFoundException || ex is ItemNotFoundException)
                return NotFound();
            else if (ex is ItemTypeException)
                return BadRequest();
            throw;
        }
    }

    [HttpPatch]
    public async Task<IActionResult> RenameAsync(string? id, string? newName)
    {
        if (id == null || newName == null)
            return BadRequest();
        
        var subject = await _subjectStorageService.GetAsync(id);
        if (subject == null)
            return NotFound();

        var anotherSubject = await _subjectStorageService.GetByGroupAndNameAsync(subject.GroupId, newName);
        if (anotherSubject != null)
            return BadRequest();

        try
        {
            await _fileSystemService.RenameAsync(subject.Id, newName);
        }
        catch (ItemNotFoundException)
        {
            return NotFound();
        }

        subject.Name = newName;
        await _subjectStorageService.UpdateAsync(subject);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(string? id)
    {
        if (id == null)
            return BadRequest();
        
        var subject = await _subjectStorageService.GetAsync(id);
        if (subject == null)
            return NotFound();

        try
        {
            await _fileSystemService.RemoveFolder(subject.Id);
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
        await _subjectStorageService.DeleteAsync(subject.Id);

        return Ok();
    }
}