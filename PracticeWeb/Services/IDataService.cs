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
    /// <returns></returns>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Получить сущность
    /// </summary>
    /// <param name="id">ИД сущности</param>
    /// <returns>Сущность</returns>
    Task<T?> GetAsync(string id);

    /// <summary>
    /// Получить все сущности
    /// </summary>
    /// <returns>Список сущностей</returns>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// Обновить сущность
    /// </summary>
    /// <param name="id">ИД сущности</param>
    /// <param name="entity">Сущность</param>
    /// <returns></returns>
    Task UpdateAsync(string id, T entity);

    /// <summary>
    /// Удалить сущность
    /// </summary>
    /// <param name="id">ИД сущности</param>
    /// <returns></returns>
    Task DeleteAsync(string id);
}