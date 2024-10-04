using Newtonsoft.Json;

namespace TravelAgency.DataProcessor.ExportDtos
{
    public class CustomerExportDto
    {
        [JsonProperty("FullName")]
        public string FullName { get; set; }

        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("Bookings")]
        public BookingExportDto[] Bookings { get; set; }
    }
}
