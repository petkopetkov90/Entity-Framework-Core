using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgency.Data.Models
{
    public class TourPackageGuide
    {
        [Required]
        public int TourPackageId { get; set; }
        [ForeignKey(nameof(TourPackageId))]
        public TourPackage TourPackage { get; set; }

        [Required]
        public int GuideId { get; set; }
        [ForeignKey(nameof(GuideId))]
        public Guide Guide { get; set; }
    }
}
