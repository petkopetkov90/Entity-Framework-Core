﻿using System.Xml.Serialization;

namespace ProductShop.DTOs.Import
{
    [XmlType("CategoryProduct")]
    public class CategoryProductImportDto
    {
        [XmlElement("CategoryId")]
        public int? CategoryId { get; set; }
        [XmlElement("ProductId")]
        public int? ProductId { get; set; }
    }
}
