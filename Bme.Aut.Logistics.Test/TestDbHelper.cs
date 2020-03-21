using Bme.Aut.Logistics.Dal;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Bme.Aut.Logistics.Test
{
    public static class TestDbHelper
    {
        public static SqliteConnection CreateConnection()
        {
            var conn = new SqliteConnection(@"DataSource=:memory:");
            conn.Open();
            return conn;
        }

        public static LogisticsDbContext CreateDbContext(SqliteConnection connection)
        {
            var dbContextOptions = new DbContextOptionsBuilder<LogisticsDbContext>()
                                    .UseSqlite(connection)
                                    .Options;
            var dbContext = new LogisticsDbContext(dbContextOptions);
            dbContext.Database.EnsureCreated();
            return dbContext;
        }
    }

}
