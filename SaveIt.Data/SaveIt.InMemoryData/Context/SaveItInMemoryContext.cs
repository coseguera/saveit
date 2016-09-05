using System;
using System.Collections.Generic;
using SaveIt.Common;

namespace SaveIt.InMemoryData.Context
{
    public class SaveItInMemoryContext
    {
        public SaveItInMemoryContext()
        {
            this.Users = new List<SaveItUser>();
            this.Accounts = new List<Account>();
            this.Categories = new List<Category>();
            this.People = new List<Person>();
            this.AccountTransactions = new List<AccountTransaction>();
        }

        public List<SaveItUser> Users { get; private set; }

        public List<Account> Accounts { get; private set; }

        public List<Category> Categories { get; private set; }

        public List<Person> People { get; private set; }

        public List<AccountTransaction> AccountTransactions { get; private set; }

        public List<T> GetEntitySet<T>() where T : SaveItEntityBase
        {
            if (typeof(T) == typeof(Account))
            {
                return Accounts as List<T>;
            }
            if (typeof(T) == typeof(Category))
            {
                return Categories as List<T>;
            }
            if (typeof(T) == typeof(Person))
            {
                return People as List<T>;
            }
            if (typeof(T) == typeof(AccountTransaction))
            {
                return AccountTransactions as List<T>;
            }

            throw new NotImplementedException();
        }
    }
}