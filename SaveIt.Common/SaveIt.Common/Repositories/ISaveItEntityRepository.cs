using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaveIt.Common.Repositories
{
    public interface ISaveItEntityRepository<T> where T : SaveItEntityBase
    {
        Task<IEnumerable<T>> GetAllAsync(Guid userId);

        Task<T> GetAsync(Guid userId, Guid id);

        Task CreateAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(Guid userId, Guid id);
    }
}