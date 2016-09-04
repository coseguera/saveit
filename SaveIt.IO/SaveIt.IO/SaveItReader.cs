using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SaveIt.Common;

namespace SaveIt.IO
{
    public static class SaveItReader
    {
        public static IEnumerable<SaveItUser> ReadUsers(string path)
        {
            foreach (string line in File.ReadAllLines(path))
            {
                string[] parts = line.Split('\t');

                if (parts[0].Equals(typeof(SaveItUser).ToString()))
                {
                    SaveItUser user = JsonConvert.DeserializeObject<SaveItUser>(parts[1]);
                    yield return user;
                }
            }
        }

        public static IEnumerable<T> ReadEntities<T>(string path)
            where T : SaveItEntityBase
        {
            foreach (string line in File.ReadAllLines(path))
            {
                string[] parts = line.Split('\t');

                if (parts[0].Equals(typeof(T).ToString()))
                {
                    T entity = JsonConvert.DeserializeObject<T>(parts[1]);
                    yield return entity;
                }
            }
        }

        public static IEnumerable<AccountTransaction> ReadInputFile(string path, SaveItUser saveitUser, Account account, Category category, IList<Person> people)
        {
            foreach (string line in File.ReadAllLines(path))
            {
                string[] parts = line.Split(',');
                if (!parts.All(p => string.IsNullOrWhiteSpace(p)))
                {
                    string concept = parts[2];
                    decimal total = decimal.Parse(parts[3]);
                    AccountTransaction accountTransaction = new AccountTransaction
                    {
                        SaveItUserId = saveitUser.Id,
                        SaveItUser = saveitUser,
                        Date = DateTime.Parse(parts[0]),
                        BankDescription = parts[1],
                        AccountId = account.Id,
                        Account = account,
                        Transactions = new List<Transaction>()
                    };

                    for (int i = 4; i < parts.Length; i++)
                    {
                        string partValue = parts[i];
                        Person person = people[i - 4];
                        decimal amount;
                        if (!string.IsNullOrWhiteSpace(partValue) && decimal.TryParse(partValue, out amount))
                        {
                            Transaction transaction = new Transaction
                            {
                                SaveItUserId = saveitUser.Id,
                                SaveItUser = saveitUser,
                                Amount = amount,
                                Concept = concept,
                                PersonId = person.Id,
                                Person = person,
                                CategoryId = category.Id,
                                Category = category,
                                AccountTransactionId = accountTransaction.Id,
                            };

                            accountTransaction.Transactions.Add(transaction);
                        }
                    }

                    decimal calculatedTotal = accountTransaction.Transactions.Sum(t => t.Amount);

                    if (total != calculatedTotal)
                    {
                        throw new Exception("Data is invalid");
                    }

                    yield return accountTransaction;
                }
            }
        }
    }
}