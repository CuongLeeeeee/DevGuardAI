using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration;
namespace DevGuardAI.DAL.Data
{
    public class DevGuardAIDbContextFactory
        : IDesignTimeDbContextFactory<DevGuardAIDbContext>
    {
        public DevGuardAIDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DevGuardAIDbContext>();

            // Load config từ API project
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../DevGuardAI.API"))
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new DevGuardAIDbContext(optionsBuilder.Options);
        }
    }
}