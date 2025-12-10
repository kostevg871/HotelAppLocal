using HotelAppLocal.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAppLocal.Data
{
    public class AppDbContext : DbContext
    {
        private readonly string _dbPath;

        public AppDbContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        public DbSet<Room> Rooms => Set<Room>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // SQLite-файл лежит по пути _dbPath
            optionsBuilder.UseSqlite($"Filename={_dbPath}");
        }
    }
}
