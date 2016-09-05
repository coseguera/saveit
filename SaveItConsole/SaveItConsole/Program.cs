using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SaveIt.Common;
using SaveIt.InMemoryData;
using SaveIt.IO;

namespace SaveItConsole
{
    public class Program
    {
        private const string entityFileRelativePath = "../saveitdata/saveitentities.txt";
        private const string inputFileRelativePath = "../saveitdata/input/rawAug15.csv";

        public static void Main(string[] args)
        {
            ISaveItService service = GetInMemoryService().Result;
            PrintData(service).Wait();
        }

        private static string GetFilePath(string fileRelativePath)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string path = Path.Combine(currentDirectory, fileRelativePath);
            return path;
        }

        private static async Task<ISaveItService> GetInMemoryService()
        {
            string filePath = GetFilePath(entityFileRelativePath);
            string inputFilePath = GetFilePath(inputFileRelativePath);
            ISaveItService service = new SaveItInMemoryService();

            List<SaveItUser> users = SaveItReader.ReadUsers(filePath).ToList();
            foreach(var user in users)
            {
                await service.Users.CreateAsync(user);
            }

            List<Account> accounts = SaveItReader.ReadEntities<Account>(filePath).ToList();
            foreach(var account in accounts)
            {
                await service.Accounts.CreateAsync(account);
            }

            List<Category> categories = SaveItReader.ReadEntities<Category>(filePath).ToList();
            foreach(var category in categories)
            {
                await service.Categories.CreateAsync(category);
            }

            List<Person> people = SaveItReader.ReadEntities<Person>(filePath).ToList();
            foreach(var person in people)
            {
                await service.People.CreateAsync(person);
            }

            var usr = users.Single();
            var ftc = accounts.Single(a => a.Name.Equals("ftc", StringComparison.OrdinalIgnoreCase));
            var cat = categories.Single();

            List<AccountTransaction> accountTransactions = SaveItReader.ReadInputFile(inputFilePath, usr, ftc, cat, people).ToList();
            foreach(var accountTransaction in accountTransactions)
            {
                await service.AccountTransactions.CreateAsync(accountTransaction);
            }
            return service;
        }

        private static async Task PrintData(ISaveItService service)
        {
            var user = (await service.Users.GetAllAsync()).Single();
            Console.WriteLine("User:");
            Console.WriteLine(JsonConvert.SerializeObject(user, Formatting.Indented));

            var accounts = await service.Accounts.GetAllAsync(user.Id);
            Console.WriteLine("Accounts:");
            Console.WriteLine(JsonConvert.SerializeObject(accounts, Formatting.Indented));

            var category = (await service.Categories.GetAllAsync(user.Id)).Single();
            Console.WriteLine("Category:");
            Console.WriteLine(JsonConvert.SerializeObject(category, Formatting.Indented));

            var people = await service.People.GetAllAsync(user.Id);
            Console.WriteLine("People:");
            Console.WriteLine(JsonConvert.SerializeObject(people, Formatting.Indented));

            var accountTransactions = await service.AccountTransactions.GetAllAsync(user.Id);

            var sampleTransaction = accountTransactions.Where(x => x.Transactions.Sum(t => t.Amount) > 0).First();

            Console.WriteLine("Paychecks:");
            Console.WriteLine(JsonConvert.SerializeObject(sampleTransaction, Formatting.Indented));

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
    }
}
