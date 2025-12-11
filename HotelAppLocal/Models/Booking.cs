using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAppLocal.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Room))]
        public int RoomId { get; set; }

        public Room Room { get; set; } = null!;

        /// <summary>
        /// Имя гостя
        /// </summary>
        [MaxLength(100)]
        public string GuestName { get; set; } = null!;

        /// <summary>
        /// Дата заезда (включительно)
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Дата выезда ( *не* включая эту дату)
        /// </summary>
        public DateTime ToDate { get; set; }
    }
}
