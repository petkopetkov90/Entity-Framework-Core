using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace TravelAgency.DataProcessor.ImportDtos
{
    [XmlType("Customer")]
    public class CustomerImportDto
    {
        [Required]
        [MaxLength(60)]
        [MinLength(4)]
        [XmlElement("FullName")]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [MinLength(6)]
        [XmlElement("Email")]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(13)]
        [MinLength(13)]
        [RegularExpression(@"^\+\d{12}$")]
        [XmlAttribute("phoneNumber")]
        public string PhoneNumber { get; set; } = null!;
    }
}
