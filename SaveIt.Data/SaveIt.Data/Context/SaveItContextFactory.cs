namespace SaveIt.Data.Context
{
    using Microsoft.EntityFrameworkCore;

    public class SaveItContextFactory
    {
        public static SaveItContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SaveItContext>();
            optionsBuilder.UseMySql(connectionString);

            // Ensure database creation
            var context = new SaveItContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            return context;
        }
    }
}