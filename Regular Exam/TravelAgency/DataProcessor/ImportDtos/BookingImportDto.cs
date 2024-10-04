using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TravelAgency.DataProcessor.ImportDtos
{
    public class BookingImportDto
    {
        [Required]
        [JsonProperty("BookingDate")]
        public string BookingDate { get; set; }

        [JsonProperty("CustomerName")]
        [Required]
        public string CustomerName { get; set; }

        [JsonProperty("TourPackageName")]
        [Required]
        public string TourPackageName { get; set; }
    }
}
