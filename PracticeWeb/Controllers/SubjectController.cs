using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;
using PracticeWeb.Services.FileSystemServices;
using PracticeWeb.Services.SubjectStorageServices;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectController : ControllerBase
{
    private IFileSystemService _fileSystemService;
    private ISubjectStorageService _subjectStorageService;

    public SubjectController(IFileSystemService fileSystemService, ISubjectStorageService subjectStorageService)
    {
        _fileSystemService = fileSystemService;
        _subjectStorageService = subjectStorageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(string? id)
    {
        if (id == null)
            return BadRequest();

        var groupInfo = await _subjectStorageService.GetAsync(id);
        if (groupInfo == null)
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
}