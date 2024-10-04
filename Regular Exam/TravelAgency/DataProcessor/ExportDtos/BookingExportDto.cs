using Newtonsoft.Json;

namespace TravelAgency.DataProcessor.ExportDtos
{
    public class BookingExportDto
    {
        [JsonProperty("TourPackageName")]
        public string TourPackageName { get; set; }

        [JsonProperty("Date")]
        public string Date { get; set; }
    }
}
