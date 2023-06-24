using PracticeWeb.Models;

namespace PracticeWeb.Services;

public interface IDataService<TId, T>
    where TId : IEquatable<TId>
    where T : CommonModel<TId>
{
    /// <summary>
    /// Добавить сущность
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <returns></returns>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Получить сущность
    /// </summary>
    /// <param name="id">ИД сущности</param>
    /// <returns>Сущность</returns>
    Task<T?> GetAsync(TId id);

    /// <summary>
    /// Получить все сущности
    /// </summary>
    /// <returns>Список сущностей</returns>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// Обновить сущность
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <returns></returns>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Удалить сущность
    /// </summary>
    /// <param name="id">ИД сущности</param>
    /// <returns></returns>
    Task DeleteAsync(TId id);
}
