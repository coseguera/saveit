using System;
using Microsoft.EntityFrameworkCore;
using SaveIt.Common;

namespace SaveIt.Data.Context
{
    public class SaveItContext: DbContext
    {
        public SaveItContext(DbContextOptions<SaveItContext> options) : base(options)
        {
        }

        public DbSet<SaveItUser> Users { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Person> People { get; set; }

        public DbSet<AccountTransaction> AccountTransactions { get; set; }

        public static Action<DbContextOptionsBuilder> UseMySql(string connectionString)
        {
            Action<DbContextOptionsBuilder> useMySql = o => o.UseMySql(connectionString);
            return useMySql;
        }
    }
}