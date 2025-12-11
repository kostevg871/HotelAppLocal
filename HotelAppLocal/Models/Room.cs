using System.ComponentModel.DataAnnotations;

namespace HotelAppLocal.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        public string Number { get; set; } = null!; // номер комнаты

        public bool IsAvailable { get; set; } = true; // доступна ли для бронирования

        public float PricePerNight { get; set; } = 0;
    }
}
