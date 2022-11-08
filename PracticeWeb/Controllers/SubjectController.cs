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
            return BadRequest();

        try
        {
            return new JsonResult(await _fileSystemService.GetFolderInfoAsync(id));
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
            return BadRequest();

        try
        {
            Item folder = await _fileSystemService.CreateFolderAsync(group.Id, name, 4);
            var subject = new Subject
            {
                Id = folder.Id,
                GroupId = group.Id,
                Name = name,
                Description = description
            };
            await _subjectStorageService.CreateAsync(subject);
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

    [HttpPatch]
    public async Task<IActionResult> RenameAsync(string? id, string? newName)
    {
        if (id == null || newName == null)
            return BadRequest();
        
        var subject = await _subjectStorageService.GetAsync(id);
        if (subject == null)
            return BadRequest();

        try
        {
            await _fileSystemService.RenameAsync(subject.Id, newName);
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
            subject.Name = newName;
            await _subjectStorageService.UpdateAsync(subject.Id, subject);
        }

        return Ok();
    }
}