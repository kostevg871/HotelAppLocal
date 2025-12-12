using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySqlConnector; // убедись, что этот using есть

namespace HotelAppLocal.Data
{
    public class DbHealthService
    {
        private readonly AppDbContext _db;

        public Exception? LastError { get; private set; }

        public DbHealthService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Проверка доступности сервера/порта/логина. 
        /// Базу может не находить (Unknown database) — это считаем успехом.
        /// </summary>
        public async Task<bool> CheckConnectionAsync()
        {
            try
            {
                LastError = null;

                var connString = _db.Database.GetConnectionString();
                await using var conn = new MySqlConnection(connString);
                await conn.OpenAsync(); // если базы нет, может выкинуть Unknown database

                return true;
            }
            catch (MySqlException ex)
            {
                // Unknown database -> сервер доступен, это нас устраивает для теста порта
                if (ex.Number == 1049) // 1049 = Unknown database
                {
                    return true;
                }

                LastError = ex;
                return false;
            }
            catch (Exception ex)
            {
                LastError = ex;
                return false;
            }
        }

        /// <summary>
        /// Пересоздать тестовую базу (EnsureDeleted + EnsureCreated + тестовые данные).
        /// </summary>
        public async Task<bool> InitializeTestDatabaseAsync()
        {
            try
            {
                LastError = null;

                DbInitializer.Initialize(_db);

                // на всякий случай проверим соединение
                await _db.Database.CanConnectAsync();
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex;
                return false;
            }
        }
    }
}
