using Microsoft.EntityFrameworkCore;
using PracticeWeb.Models;

namespace PracticeWeb.Services.SubjectStorageServices;

public class SubjectStorageService : ISubjectStorageService
{
    private Context _context;
    private CommonQueries<Subject> _common;

    public SubjectStorageService(Context context)
    {
        _context = context;
        _common = new CommonQueries<Subject>(_context);
    }

    public async Task<Subject> CreateAsync(Subject entity) =>
        await _common.CreateAsync(entity);

    public async Task<Subject?> GetAsync(string id) =>
        await _common.GetAsync(id, IncludeValues());

    public async Task<List<Subject>> GetAllAsync() =>
        await _common.GetAllAsync(IncludeValues());

    public async Task<List<Subject>> GetByGroupAsync(string groupId) =>
        await IncludeValues().Where(s => s.GroupId == groupId).ToListAsync();

    public async Task UpdateAsync(string id, Subject entity) =>
        await _common.UpdateAsync(id, entity);

    public async Task DeleteAsync(string id) =>
        await _common.DeleteAsync(id);

    private IQueryable<Subject> IncludeValues() =>
        _context.Subjects
            .Include(s => s.Group);
}