using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApp.Types.Repositories
{
    public interface IRepository<T, in TPrimaryKey>
    {
        Task Add(T model);
        Task AddMany(params T[] models);
        Task AddMany(IEnumerable<T> models);
        
        Task<IEnumerable<T>> GetAll();
        Task<T> Get(TPrimaryKey id);
        Task<IEnumerable<T>> GetMany(params TPrimaryKey[] ids);
        Task<IEnumerable<T>> GetMany(IEnumerable<TPrimaryKey> ids);

        Task Edit(T model);

        Task Delete(T model);
        Task DeleteMany(params T[] models);
        Task DeleteMany(IEnumerable<T> models);
        Task Delete(TPrimaryKey key);
        Task DeleteMany(params TPrimaryKey[] keys);
        Task DeleteMany(IEnumerable<TPrimaryKey> keys);
    }
}