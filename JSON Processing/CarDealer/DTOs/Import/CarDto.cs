namespace CarDealer.DTOs.Import
{
    public class CarDto
    {
        public string Make { get; set; } = null!;

        public string Model { get; set; } = null!;

        public long TraveledDistance { get; set; }

        public List<int> PartsId { get; set; }
    }
}
