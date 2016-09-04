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
        }

        public List<SaveItUser> Users { get; private set; }

        public List<Account> Accounts { get; private set; }

        public List<T> GetEntitySet<T>() where T : SaveItEntityBase
        {
            if(typeof(T) == typeof(Account))
            {
                return Accounts as List<T>;
            }

            throw new NotImplementedException();
        }
    }
}