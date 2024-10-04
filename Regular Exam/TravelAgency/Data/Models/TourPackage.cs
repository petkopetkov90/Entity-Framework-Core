using System.ComponentModel.DataAnnotations;

namespace TravelAgency.Data.Models
{
    public class TourPackage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string PackageName { get; set; } = null!;

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        //range for positive?
        public decimal Price { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public virtual ICollection<TourPackageGuide> TourPackagesGuides { get; set; } = new List<TourPackageGuide>();

    }
}
