using System.Linq.Expressions;

namespace VillaAPI.Repository.IRepository;

public interface IRepository<T> where T : class
{
    Task CreateAsync(T entity);
    Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    Task RemoveAsync(T entity);
    Task SaveAsync();
}