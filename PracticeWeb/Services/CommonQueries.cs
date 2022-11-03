using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PracticeWeb.Models;

namespace PracticeWeb.Services;

public class CommonQueries<T> where T : CommonModel
{
    private Context _context;

    public CommonQueries(Context context)
    {
        _context = context;
    }

    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken)
    {
        var created = await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();

        return created.Entity;
    }

    public async Task<T?> GetAsync(
        string id, 
        IQueryable<T> collection, 
        CancellationToken cancellationToken) => 
            await collection.FirstOrDefaultAsync(e => e.Id == id);

    public async Task<List<T>> GetAllAsync(
        IQueryable<T> collection,
        CancellationToken cancellationToken) =>
            await collection.ToListAsync();

    public async Task UpdateAsync(string id, T entity, CancellationToken cancellationToken)
    {
        entity.Id = id;
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await _context.Set<T>().FirstOrDefaultAsync((e) => e.Id == id);
        if (entity is null)
            throw new NullReferenceException();
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}