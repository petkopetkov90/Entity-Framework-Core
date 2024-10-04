using System.Xml.Serialization;

namespace ProductShop.DTOs.Export
{
    public class UsersWithCountExportDto
    {
        [XmlElement("count")]
        public int Count { get; set; }
        [XmlArray("users")]
        public List<UserAndProductExportDto> UserAndProducts { get; set; }
    }
}
