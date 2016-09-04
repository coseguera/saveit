using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SaveIt.Common;
using SaveIt.IO;

namespace SaveItConsole
{
    public class Program
    {
        private const string entityFileRelativePath = "saveitentities.txt";
        private const string inputFileRelativePath = "input/rawAug15.csv";

        public static void Main(string[] args)
        {
            string filePath = GetFilePath(entityFileRelativePath);
            List<SaveItUser> users = SaveItReader.ReadUsers(filePath).ToList();
            List<Account> accounts = SaveItReader.ReadEntities<Account>(filePath).ToList();
            List<Category> categories = SaveItReader.ReadEntities<Category>(filePath).ToList();
            List<Person> people = SaveItReader.ReadEntities<Person>(filePath).ToList();

            Console.WriteLine("Users:");
            Console.WriteLine(JsonConvert.SerializeObject(users, Formatting.Indented));

            Console.WriteLine("Accounts:");
            Console.WriteLine(JsonConvert.SerializeObject(accounts, Formatting.Indented));

            Console.WriteLine("Categories:");
            Console.WriteLine(JsonConvert.SerializeObject(categories, Formatting.Indented));

            Console.WriteLine("People:");
            Console.WriteLine(JsonConvert.SerializeObject(people, Formatting.Indented));

            SaveItUser user = users.Single();
            Account ftc = accounts.Single(a => a.Name.Equals("ftc", StringComparison.OrdinalIgnoreCase));
            Category category = categories.Single();

            string inputFilePath = GetFilePath(inputFileRelativePath);
            List<AccountTransaction> accountTransactions = SaveItReader.ReadInputFile(inputFilePath, user, ftc, category, people).ToList();

            // var transactions = accountTransactions.SelectMany(t => t.Transactions);

            // var totalsByPerson = transactions
            //                 .GroupBy(t => t.PersonId)
            //                 .Select(g => new
            //                 {
            //                     Name = g.First().Person.Name,
            //                     Total = g.Sum(x => x.Amount)
            //                 }).ToList();

            // Console.WriteLine("Totals by Person:");
            // Console.WriteLine(JsonConvert.SerializeObject(totalsByPerson, Formatting.Indented));

            var paycheckTransactions = accountTransactions
                .Where(x => x.Transactions.Sum(t => t.Amount) > 0)
                // .Select(x => new
                // {
                //     Id = x.Id,
                //     Date = x.Date,
                //     BankDesc = x.BankDescription,
                //     AccountId = x.AccountId,
                //     Account = x.Account,
                //     Concept = x.Transactions.First().Concept,
                //     Total = x.Transactions.Sum(t => t.Amount),
                //     transactions = x.Transactions,
                // })
                // .Where(s => !s.Concept.Equals("nomina", StringComparison.OrdinalIgnoreCase))
                // .OrderBy(s => s.Date);
                .First();

            Console.WriteLine("Paychecks:");
            Console.WriteLine(JsonConvert.SerializeObject(paycheckTransactions, Formatting.Indented));
        }

        public static void CreateEntityFile(string[] args)
        {
            Guid userId = new Guid("1333ca66-0252-444a-aae1-64bd1ca5013a");
            SaveItUser user = new SaveItUser { Id = userId, Name = "coseguera" };
            string[] accounts = { "BOA", "FTS", "FTC" };
            string[] people = { "Comida", "Carlos", "Liz", "Niños", "Servicios", "Inversion", "Proyectos", "Deudas", "Apoyo", "Educacion", "Vacaciones", "Medico", "Casa", "Seguros", "Muebles" };

            string filePath = GetFilePath(entityFileRelativePath);
            SaveItWriter writer = new SaveItWriter(filePath);

            writer.WriteUserAsync(user).Wait();

            Account account;

            foreach (string name in accounts)
            {
                account = new Account { Name = name, SaveItUserId = userId };
                writer.WriteEntityAsync(account).Wait();
            }

            Category category = new Category { Name = "Unfiled", SaveItUserId = userId };
            writer.WriteEntityAsync(category).Wait();

            Person person;

            foreach (string name in people)
            {
                person = new Person { Name = name, SaveItUserId = userId };
                writer.WriteEntityAsync(person).Wait();
            }

            writer.Dispose();
        }

        private static string GetFilePath(string fileRelativePath)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string path = Path.Combine(currentDirectory, fileRelativePath);
            return path;
        }
    }
}
