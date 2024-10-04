using System.ComponentModel.DataAnnotations;

namespace TravelAgency.Data.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(60)]
        [MinLength(4)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [MinLength(6)]

        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(13)]
        [MinLength(13)]
        [RegularExpression(@"^\+\d{12}$")]
        public string PhoneNumber { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
