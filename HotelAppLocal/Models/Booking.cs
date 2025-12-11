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

        /// <summary>Имя гостя</summary>
        [MaxLength(100)]
        public string GuestName { get; set; } = null!;

        /// <summary>Телефон гостя</summary>
        [MaxLength(30)]
        public string GuestPhone { get; set; } = null!;

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        /// <summary>Статус брони</summary>
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        /// <summary>Кем создана бронь (пользователь системы)</summary>
        [ForeignKey(nameof(CreatedByUser))]
        public int? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        /// <summary>Кем подтверждена/отменена</summary>
        [ForeignKey(nameof(HandledByUser))]
        public int? HandledByUserId { get; set; }
        public User? HandledByUser { get; set; }
    }
}
