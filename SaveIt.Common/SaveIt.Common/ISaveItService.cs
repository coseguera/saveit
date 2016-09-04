using SaveIt.Common.Repositories;

namespace SaveIt.Common
{
    public interface ISaveItService
    {
        ISaveItUserRepository Users { get; set; }

        ISaveItEntityRepository<Account> Accounts { get; set; }

        ISaveItEntityRepository<AccountTransaction> AccountTransactions { get; set; }

        ISaveItEntityRepository<Category> Categories { get; set; }

        ISaveItEntityRepository<Person> People { get; set; }

        ISaveItEntityRepository<Transaction> Transactions { get; set; }
    }
}