using Microsoft.EntityFrameworkCore;
using ClassConnect.Models;

namespace ClassConnect.Services;

public class CommonQueries<TId, T>
    where TId : IEquatable<TId>
    where T : CommonModel<TId>
{
    private Context _context;

    public CommonQueries(Context context)
    {
        _context = context;
    }

    public async Task<T> CreateAsync(T entity)
    {
        var created = await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();

        return created.Entity;
    }

    public async Task<T?> GetAsync(TId id, IQueryable<T> collection) =>
        await collection.FirstOrDefaultAsync(e => Equals(e.Id, id));

    public async Task<List<T>> GetAllAsync(IQueryable<T> collection) =>
        await collection.ToListAsync();

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TId id)
    {
        var entity = await _context.Set<T>().FirstOrDefaultAsync((e) => Equals(e.Id, id));
        if (entity is null)
            throw new NullReferenceException();
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}
