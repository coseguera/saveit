using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaveIt.Common;
using SaveIt.Common.Repositories;
using SaveIt.InMemoryData.Context;

namespace SaveIt.InMemoryData.Repositories
{
    public class SaveItEntityInMemoryRepository<T> : ISaveItEntityRepository<T>  where T : SaveItEntityBase
    {
        private SaveItInMemoryContext db;

        public SaveItEntityInMemoryRepository(SaveItInMemoryContext db)
        {
            this.db = db;
        }

        public async Task CreateAsync(T entity)
        {
            var existing = await this.GetAsync(entity.Id);

            if (existing != null)
            {
                throw new InvalidOperationException(
                    string.Format("The {0} with user id {1} and id {2} already exists", typeof(T), entity.SaveItUserId, entity.Id));
            }

            db.GetEntitySet<T>().Add(entity);
        }

        public async Task DeleteAsync(Guid userId, Guid id)
        {
            var existing = await this.GetAsync(userId, id);

            if (existing == null)
            {
                throw new InvalidOperationException(
                    string.Format("The {0} with user id {1} and id {2} does not exist", typeof(T), userId, id));
            }

            db.GetEntitySet<T>().Remove(existing);
        }

        public async Task UpdateAsync(T entity)
        {
            await this.DeleteAsync(entity.SaveItUserId, entity.Id);
            await this.CreateAsync(entity);
        }

        public Task<IEnumerable<T>> GetAllAsync(Guid userId)
        {
            IEnumerable<T> entities = db.GetEntitySet<T>().Where(x => x.SaveItUserId == userId);
            return Task.FromResult(entities);
        }

        public Task<T> GetAsync(Guid userId, Guid id)
        {
            T entity = db.GetEntitySet<T>().SingleOrDefault(x => x.SaveItUserId == userId && x.Id == id);
            return Task.FromResult(entity);
        }

        private Task<T> GetAsync(Guid id)
        {
            T entity = db.GetEntitySet<T>().SingleOrDefault(x => x.Id == id);
            return Task.FromResult(entity);
        }
    }
}