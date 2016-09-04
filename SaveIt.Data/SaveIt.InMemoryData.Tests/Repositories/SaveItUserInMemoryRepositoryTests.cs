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
    public class SaveItUserInMemoryRepositoryTests
    {
        private SaveItInMemoryContext context;
        private SaveItUserInMemoryRepository repository;

        public SaveItUserInMemoryRepositoryTests()
        {
            this.context = new SaveItInMemoryContext();
            this.repository = new SaveItUserInMemoryRepository(context);
        }

        public class CreateAsync : SaveItUserInMemoryRepositoryTests
        {
            [Fact]
            public async Task CreatesUser()
            {
                var user = new SaveItUser { Name = "testUser" };

                await repository.CreateAsync(user);

                context.Users.Should().NotBeEmpty();
                context.Users.Count.Should().Be(1);
                context.Users[0].Id.Should().NotBe(Guid.Empty);
                user.Id.Should().NotBe(Guid.Empty);
                context.Users[0].Id.Should().Be(user.Id);
            }

            [Fact]
            public async Task RespectsUserId()
            {
                var id = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                var user = new SaveItUser { Id = id, Name = "testUser" };

                await repository.CreateAsync(user);

                context.Users.Should().NotBeEmpty();
                context.Users.Count.Should().Be(1);
                context.Users[0].Id.Should().Be(id);
                user.Id.Should().Be(id);
                context.Users[0].Id.Should().Be(user.Id);
            }

            [Fact]
            public void ThrowsIfUserNameExists()
            {
                var existingUser = new SaveItUser { Name = "existingUser" };
                var user = new SaveItUser { Name = "existingUser" };

                context.Users.Add(existingUser);

                Func<Task> act = async () => await repository.CreateAsync(user);
                act.ShouldThrow<InvalidOperationException>()
                    .Where(e => e.Message.Contains("existingUser"));
            }

            [Fact]
            public void ThrowsIfUserIdExists()
            {
                var existingId = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                var existingUser = new SaveItUser { Id = existingId, Name = "existingUser" };
                var id = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                var user = new SaveItUser { Id = id, Name = "testUser" };

                context.Users.Add(existingUser);

                Func<Task> act = async () => await repository.CreateAsync(user);
                act.ShouldThrow<InvalidOperationException>()
                    .Where(e => e.Message.Contains("57a04ab4-b614-4d83-bf38-8063791c63f2"));
            }
        }

        public class DeleteAsync : SaveItUserInMemoryRepositoryTests
        {
            [Fact]
            public async Task DeletesUser()
            {
                var existingId = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                var existingUser = new SaveItUser { Id = existingId, Name = "existingUser" };

                context.Users.Add(existingUser);

                await repository.DeleteAsync(new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2"));

                context.Users.Should().BeEmpty();
            }

            [Fact]
            public void ThrowsIfUserDoesNotExist()
            {
                Func<Task> act = async () => await repository.DeleteAsync(new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2"));
                act.ShouldThrow<InvalidOperationException>()
                    .Where(e => e.Message.Contains("57a04ab4-b614-4d83-bf38-8063791c63f2"));
            }
        }

        public class UpdateAsync : SaveItUserInMemoryRepositoryTests
        {
            [Fact]
            public async Task UpdatesUser()
            {
                var existingId = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                var existingUser = new SaveItUser { Id = existingId, Name = "existingUser" };

                context.Users.Add(existingUser);

                var user = new SaveItUser { Id = existingId, Name = "newName" };

                await repository.UpdateAsync(user);

                context.Users.Count.Should().Be(1);
                context.Users[0].Id.Should().Equals(new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2"));
                context.Users[0].Name.Should().Equals("newName");
            }

            [Fact]
            public void ThrowsIfUserDoesNotExist()
            {
                var id = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");

                Func<Task> act =
                    async () => await repository.UpdateAsync(
                        new SaveItUser { Id = id, Name = "nonExistingUser" });
                act.ShouldThrow<InvalidOperationException>()
                    .Where(e => e.Message.Contains("57a04ab4-b614-4d83-bf38-8063791c63f2"));
            }
        }

        public class GetAllAsync : SaveItUserInMemoryRepositoryTests
        {
            [Fact]
            public async Task GetsUsers()
            {
                context.Users.Add(new SaveItUser { Name = "user1" });
                context.Users.Add(new SaveItUser { Name = "user2" });
                context.Users.Add(new SaveItUser { Name = "user3" });

                IEnumerable<SaveItUser> users = await repository.GetAllAsync();

                users.Count().Should().Be(3);
                users.Should().ContainSingle(u => u.Name.Equals("user1"));
                users.Should().ContainSingle(u => u.Name.Equals("user2"));
                users.Should().ContainSingle(u => u.Name.Equals("user3"));
            }

            [Fact]
            public async Task ShouldReturnEmptyEnumerableIfNoUsers()
            {
                IEnumerable<SaveItUser> users = await repository.GetAllAsync();
                users.Should().NotBeNull();
                users.Should().BeEmpty();
            }
        }

        public class GetAsync : SaveItUserInMemoryRepositoryTests
        {
            [Fact]
            public async Task GetsUser()
            {
                var existingId = new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2");
                context.Users.Add(new SaveItUser { Id = existingId, Name = "existingUser" });

                var user = await repository.GetAsync(new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2"));
                user.Should().NotBeNull();
                user.Name.Should().Be("existingUser");
                user.Id.Should().Equals(new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2"));
            }

            [Fact]
            public async Task ReturnsNullIfNoUser()
            {
                var user = await repository.GetAsync(new Guid("57a04ab4-b614-4d83-bf38-8063791c63f2"));
                user.Should().BeNull();
            }
        }

        public class GetByNameAsync : SaveItUserInMemoryRepositoryTests
        {
            [Fact]
            public async Task GetsUser()
            {
                context.Users.Add(new SaveItUser { Name = "existingUser" });

                var user = await repository.GetByNameAsync("existingUser");
                user.Should().NotBeNull();
                user.Name.Should().Be("existingUser");
                user.Id.Should().NotBeEmpty();
            }

            [Fact]
            public async Task ReturnsNullIfNoUser()
            {
                var user = await repository.GetByNameAsync("nonExistingUser");
                user.Should().BeNull();
            }
        }
    }
}