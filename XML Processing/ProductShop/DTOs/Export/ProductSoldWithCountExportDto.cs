using System.Xml.Serialization;

namespace ProductShop.DTOs.Export;

[XmlType("SoldProduct")]
public class ProductSoldWithCountExportDto
{
    [XmlElement("count")]
    public int Count { get; set; }
    [XmlArray("products")]
    public List<ProductSoldExportDto> SoldProducts { get; set; }
}