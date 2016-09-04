using SaveIt.Common;
using SaveIt.Common.Repositories;
using SaveIt.InMemoryData.Context;
using SaveIt.InMemoryData.Repositories;

namespace SaveIt.InMemoryData
{
    public class SaveItInMemoryService : ISaveItService
    {
        public SaveItInMemoryService()
        {
            SaveItInMemoryContext context = new SaveItInMemoryContext();

            this.Users = new SaveItUserInMemoryRepository(context);
            this.Accounts = new SaveItEntityInMemoryRepository<Account>(context);
            this.AccountTransactions = new SaveItEntityInMemoryRepository<AccountTransaction>(context);
            this.Categories = new SaveItEntityInMemoryRepository<Category>(context);
            this.People = new SaveItEntityInMemoryRepository<Person>(context);
            this.Transactions = new SaveItEntityInMemoryRepository<Transaction>(context);
        }

        public ISaveItUserRepository Users { get; set; }

        public ISaveItEntityRepository<Account> Accounts { get; set; }

        public ISaveItEntityRepository<AccountTransaction> AccountTransactions { get; set; }

        public ISaveItEntityRepository<Category> Categories { get; set; }

        public ISaveItEntityRepository<Person> People { get; set; }

        public ISaveItEntityRepository<Transaction> Transactions { get; set; }
    }
}