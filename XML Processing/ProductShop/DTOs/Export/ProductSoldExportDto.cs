using System.Xml.Serialization;

namespace ProductShop.DTOs.Export;

[XmlType("Product")]
public class ProductSoldExportDto
{
    [XmlElement("name")]
    public string Name { get; set; }
    [XmlElement("price")]
    public decimal Price { get; set; }
}