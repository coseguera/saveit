using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using SaveIt.Common;
using SaveIt.InMemoryData.Context;
using SaveIt.InMemoryData.Repositories;
using Xunit;

namespace SaveIt.InMemoryDataTests.Repositories
{
    public class SaveItEntityInMemoryRepositoryTests
    {
        private SaveItInMemoryContext context;
        private SaveItEntityInMemoryRepository<Account> repository;

        public SaveItEntityInMemoryRepositoryTests()
        {
            this.context = new SaveItInMemoryContext();
            this.repository = new SaveItEntityInMemoryRepository<Account>(context);
        }

        public class CreateAsync : SaveItEntityInMemoryRepositoryTests
        {
            [Fact]
            public async Task CreatesEntity()
            {
                var userId = new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd");
                var entity = new Account
                {
                    SaveItUserId = userId,
                    Name = "testEntity"
                };

                await repository.CreateAsync(entity);

                context.Accounts.Should().NotBeEmpty();
                context.Accounts.Count.Should().Be(1);
                context.Accounts[0].Id.Should().NotBe(Guid.Empty);
                entity.Id.Should().NotBe(Guid.Empty);
                context.Accounts[0].Id.Should().Be(entity.Id);
            }

            [Fact]
            public async Task RespectsEntityId()
            {
                var id = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                var userId = new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd");
                var entity = new Account
                {
                    Id = id,
                    SaveItUserId = userId,
                    Name = "testEntity"
                };

                await repository.CreateAsync(entity);

                context.Accounts.Should().NotBeEmpty();
                context.Accounts.Count.Should().Be(1);
                context.Accounts[0].Id.Should().Be(id);
                entity.Id.Should().Be(id);
                context.Accounts[0].Id.Should().Be(entity.Id);
            }

            [Fact]
            public void ThrowsIfEntityIdExists()
            {
                var existingId = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                var existingEntity = new Account
                {
                    Id = existingId,
                    SaveItUserId = Guid.NewGuid(),
                    Name = "existingEntity"
                };
                var id = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                var userId = new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd");
                var entity = new Account
                {
                    Id = id,
                    SaveItUserId = userId,
                    Name = "testEntity"
                };

                context.Accounts.Add(existingEntity);

                Func<Task> act = async () => await repository.CreateAsync(entity);
                act.ShouldThrow<InvalidOperationException>()
                    .Where(e => e.Message.Contains("57a04ab4-b614-4d83-bf38-8063791c63f2"));
            }
        }

        public class DeleteAsync : SaveItEntityInMemoryRepositoryTests
        {
            [Fact]
            public async Task DeletesEntity()
            {
                var userId = new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd");
                var existingId = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                var existingEntity = new Account
                {
                    Id = existingId,
                    SaveItUserId = userId,
                    Name = "existingEntity"
                };

                context.Accounts.Add(existingEntity);

                await repository.DeleteAsync(
                    new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd"),
                    new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2"));

                context.Accounts.Should().BeEmpty();
            }

            [Fact]
            public void ThrowsIfUserDoesNotExist()
            {
                Func<Task> act = async () =>
                    await repository.DeleteAsync(
                        new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd"),
                        new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2"));
                act.ShouldThrow<InvalidOperationException>()
                    .Where(e => e.Message.Contains("57a04ab4-b614-4d83-bf38-8063791c63f2"));
            }
        }

        public class UpdateAsync : SaveItEntityInMemoryRepositoryTests
        {
            [Fact]
            public async Task UpdatesEntity()
            {
                var userId = new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd");
                var existingId = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                var existingEntity = new Account
                {
                    Id = existingId,
                    SaveItUserId = userId,
                    Name = "oldName"
                };

                context.Accounts.Add(existingEntity);

                var id = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                var entity = new Account
                {
                    Id = id,
                    SaveItUserId = userId,
                    Name = "newName"
                };

                await repository.UpdateAsync(entity);

                context.Accounts.Count.Should().Be(1);
                context.Accounts[0].Id.Should().Equals(new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2"));
                context.Accounts[0].Name.Should().Equals("newName");
            }

            [Fact]
            public void ThrowsIfEntityDoesNotExist()
            {
                var userId = new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd");
                var id = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");

                Func<Task> act =
                    async () => await repository.UpdateAsync(
                        new Account
                        {
                            Id = id,
                            SaveItUserId = userId,
                            Name = "nonExistingEntity"
                        });
                act.ShouldThrow<InvalidOperationException>()
                    .Where(e => e.Message.Contains("57a04ab4-b614-4d83-bf38-8063791c63f2"));
            }
        }

        public class GetAllAsync : SaveItEntityInMemoryRepositoryTests
        {
            [Fact]
            public async Task GetsEntities()
            {
                var userId = new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd");
                context.Accounts.Add(new Account { Name = "account1", SaveItUserId = userId });
                context.Accounts.Add(new Account { Name = "account2", SaveItUserId = userId });
                context.Accounts.Add(new Account { Name = "account3", SaveItUserId = userId });

                IEnumerable<Account> entities = await repository.GetAllAsync(userId);

                entities.Count().Should().Be(3);
                entities.Should().ContainSingle(u => u.Name.Equals("account1"));
                entities.Should().ContainSingle(u => u.Name.Equals("account2"));
                entities.Should().ContainSingle(u => u.Name.Equals("account3"));
            }

            [Fact]
            public async Task ShouldReturnEmptyEnumerableIfNoEntities()
            {
                var userId = new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd");
                IEnumerable<Account> entities = await repository.GetAllAsync(userId);
                entities.Should().NotBeNull();
                entities.Should().BeEmpty();
            }
        }

        public class GetAsync : SaveItEntityInMemoryRepositoryTests
        {
            [Fact]
            public async Task GetsEntity()
            {
                var userId = new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd");
                var existingId = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                context.Accounts.Add(new Account
                {
                    Id = existingId,
                    SaveItUserId = userId,
                    Name = "existingEntity"
                });

                var entity = await repository.GetAsync(
                    new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd"),
                    new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2"));
                entity.Should().NotBeNull();
                entity.Name.Should().Be("existingEntity");
                entity.Id.Should().Equals(new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2"));
            }

            [Fact]
            public async Task ReturnsNullIfNoEntity()
            {
                var entity = await repository.GetAsync(
                    new Guid("a30b8559-7c24-4b49-80d7-d881b378d1dd"),
                    new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2"));
                entity.Should().BeNull();
            }
        }
    }
}