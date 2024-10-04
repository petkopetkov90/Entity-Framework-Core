﻿using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("Car")]
    public class CarImportDto
    {
        [XmlElement("make")]
        public string Make { get; set; }
        [XmlElement("model")]
        public string Model { get; set; }
        [XmlElement("traveledDistance")]
        public long TraveledDistance { get; set; }
        [XmlArray("parts")]
        public List<PartId> PartsIds { get; set; }
    }

    [XmlType("partId")]
    public class PartId
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
