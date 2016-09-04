using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaveIt.Common;
using SaveIt.Common.Repositories;
using SaveIt.InMemoryData.Context;

namespace SaveIt.InMemoryData.Repositories
{
    public class SaveItUserInMemoryRepository : ISaveItUserRepository
    {
        private SaveItInMemoryContext db;

        public SaveItUserInMemoryRepository(SaveItInMemoryContext db)
        {
            this.db = db;
        }

        public async Task CreateAsync(SaveItUser user)
        {
            SaveItUser existingUser = await this.GetByNameAsync(user.Name);
            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    string.Format("The user with name {0} already exists", user.Name));
            }

            existingUser = await this.GetAsync(user.Id);
            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    string.Format("The user with id {0} already exists", user.Id));
            }

            this.db.Users.Add(user);
        }

        public async Task DeleteAsync(Guid userId)
        {
            var existingUser = await this.GetAsync(userId);

            if (existingUser == null)
            {
                throw new InvalidOperationException(
                    string.Format("The user with id {0} does not exist", userId));
            }

            this.db.Users.Remove(existingUser);
        }

        public Task<IEnumerable<SaveItUser>> GetAllAsync()
        {
            return Task.FromResult(this.db.Users as IEnumerable<SaveItUser>);
        }

        public Task<SaveItUser> GetAsync(Guid userId)
        {
            var user = this.db.Users.SingleOrDefault(u => u.Id == userId);
            return Task.FromResult(user);
        }

        public Task<SaveItUser> GetByNameAsync(string name)
        {
            var user = this.db.Users.SingleOrDefault(u => u.Name == name);
            return Task.FromResult(user);
        }

        public async Task UpdateAsync(SaveItUser user)
        {
            var existingUser = await this.GetAsync(user.Id);

            if (existingUser == null)
            {
                throw new InvalidOperationException(
                    string.Format("The user with id {0} does not exist", user.Id));
            }

            existingUser.Name = user.Name;
        }
    }
}