using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaveIt.Common.Repositories
{
    public interface ISaveItUserRepository
    {
        Task<IEnumerable<SaveItUser>> GetAllAsync();

        Task<SaveItUser> GetAsync(Guid userId);

        Task<SaveItUser> GetByNameAsync(string name);

        Task CreateAsync(SaveItUser user);

        Task UpdateAsync(SaveItUser user);

        Task DeleteAsync(Guid userId);
    }
}