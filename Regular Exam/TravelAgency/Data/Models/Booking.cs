using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgency.Data.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public int CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

        [Required]
        public int TourPackageId { get; set; }
        [ForeignKey(nameof(TourPackageId))]
        public TourPackage TourPackage { get; set; }
    }
}
