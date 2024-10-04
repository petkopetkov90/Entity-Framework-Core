using System.Xml.Serialization;

namespace TravelAgency.DataProcessor.ExportDtos
{
    [XmlType("Guide")]
    public class GuideSpanishSpeakExportDtop
    {
        [XmlElement("FullName")]
        public string FullName { get; set; }

        [XmlArray("TourPackages")]
        public TourPackageDto[] TourPackages { get; set; }
    }
}
