using System.Linq.Expressions;
using VillaAPI.Models;

namespace VillaAPI.Repository.IRepository;

public interface IVillaRepository
{
    Task CreateAsync(Villa entity);
    Task<Villa> GetAsync(Expression<Func<Villa, bool>> filter = null, bool tracked = true);
    Task<List<Villa>> GetAllAsync(Expression<Func<Villa, bool>> filter = null);
    Task RemoveAsync(Villa entity);
    Task SaveAsync();
    Task UpdateAsync(Villa entity);
}