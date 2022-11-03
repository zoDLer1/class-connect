using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using PracticeWeb.Models;

namespace PracticeWeb.Services;

public interface IDataService<T>
{
    /// <summary>
    /// Добавить сущность
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Получить сущность
    /// </summary>
    /// <param name="id">ИД сущности</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Сущность</returns>
    Task<T?> GetAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Получить все сущности
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Список сущностей</returns>
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Обновить сущность
    /// </summary>
    /// <param name="id">ИД сущности</param>
    /// <param name="entity">Сущность</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateAsync(int id, T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить сущность
    /// </summary>
    /// <param name="id">ИД сущности</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}