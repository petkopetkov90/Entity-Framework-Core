using System.Xml.Serialization;

namespace TravelAgency.DataProcessor.ExportDtos
{

    [XmlType("TourPackage")]
    public class TourPackageDto
    {
        [XmlElement("Name")] public string Name { get; set; }

        [XmlElement("Description")] public string Description { get; set; }

        [XmlElement("Price")] public decimal Price { get; set; }
    }
}