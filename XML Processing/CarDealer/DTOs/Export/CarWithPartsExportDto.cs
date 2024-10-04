using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("car")]
    public class CarWithPartsExportDto
    {
        [XmlAttribute("make")]
        public string Make { get; set; }
        [XmlAttribute("model")]
        public string Model { get; set; }
        [XmlAttribute("traveled-distance")]
        public long TraveledDistance { get; set; }
        [XmlArray("parts")]
        public List<PartExportDto> Parts { get; set; }
    }

}
