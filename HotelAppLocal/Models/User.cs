using System.ComponentModel.DataAnnotations;

namespace HotelAppLocal.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [MaxLength(200)]
        public string PasswordHash { get; set; } = null!; // для демо – просто пароль

        public UserRole Role { get; set; }
    }
}
