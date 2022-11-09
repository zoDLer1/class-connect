using PracticeWeb.Models;

namespace PracticeWeb.Services.SubjectStorageServices;

public interface ISubjectStorageService : IDataService<Subject>
{
    Task<List<Subject>> GetByGroupAsync(string groupId);
    Task<Subject?> GetByGroupAndNameAsync(string groupId, string name);
}