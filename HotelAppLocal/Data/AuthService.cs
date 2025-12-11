using HotelAppLocal.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAppLocal.Data
{
    public class AuthService
    {
        private readonly AppDbContext _db;

        public User? CurrentUser { get; private set; }

        public AuthService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);

            if (user == null)
                return false;

            CurrentUser = user;
            return true;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            var exists = await _db.Users.AnyAsync(u => u.Username == username);
            if (exists)
                return false;

            var user = new User
            {
                Username = username,
                PasswordHash = password,
                Role = UserRole.User   // ⚠️ Теперь новые пользователи – обычные!
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            CurrentUser = user;
            return true;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public bool IsInRole(UserRole role) => CurrentUser?.Role == role;
    }
}
