using Bme.Aut.Logistics.Dal;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Bme.Aut.Logistics
{
    class Program
    {
        static void Main(string[] args)
        {
            // Pelda DbContext tesztelehez
            // De inkabb hasznaljuk a unit teszteket

            var conn = new SqliteConnection(@"DataSource=:memory:");
            conn.Open();
            var dbContextOptions = new DbContextOptionsBuilder<LogisticsDbContext>()
                                    .UseSqlite(conn)
                                    .Options;
            var dbContext = new LogisticsDbContext(dbContextOptions);
            dbContext.Database.EnsureCreated();
            var svc = new Service.AddressService(dbContext);
        }
    }
}
