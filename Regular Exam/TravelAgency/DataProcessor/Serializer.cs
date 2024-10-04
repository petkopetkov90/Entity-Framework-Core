using Newtonsoft.Json;
using TravelAgency.Data;
using TravelAgency.Data.Models.Enums;
using TravelAgency.DataProcessor.ExportDtos;

namespace TravelAgency.DataProcessor
{
    public class Serializer
    {
        public static string ExportGuidesWithSpanishLanguageWithAllTheirTourPackages(TravelAgencyContext context)
        {
            var guides = context.Guides
                .Where(g => g.Language == Language.Spanish)
                .OrderByDescending(g => g.TourPackagesGuides.Count)
                .ThenBy(g => g.FullName)
                .Select(g => new GuideSpanishSpeakExportDtop()
                {
                    FullName = g.FullName,
                    TourPackages = g.TourPackagesGuides
                        .Select(p => new TourPackageDto()
                        {
                            Name = p.TourPackage.PackageName,
                            Description = p.TourPackage.Description,
                            Price = p.TourPackage.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ThenBy(p => p.Name)
                        .ToArray()
                })
                .ToArray();

            return XmlSerializationHelper.Serialize(guides, "Guides");
        }

        public static string ExportCustomersThatHaveBookedHorseRidingTourPackage(TravelAgencyContext context)
        {
            var customersTemp = context.Customers
                .Where(c => c.Bookings.Any(b => b.TourPackage.PackageName == "Horse Riding Tour"))
                .Select(c => new
                {
                    FullName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    Bookings = c.Bookings
                        .Where(b => b.TourPackage.PackageName == "Horse Riding Tour")
                        .Select(b => new
                        {
                            TourPackageName = b.TourPackage.PackageName,
                            Date = b.BookingDate
                        })
                    .ToArray()
                })
                .ToArray();

            var customers = customersTemp
                .OrderByDescending(c => c.Bookings.Length)
                .ThenBy(c => c.FullName)
                .Select(c => new CustomerExportDto()
                {
                    FullName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    Bookings = c.Bookings
                        .Select(b => new BookingExportDto()
                        {
                            TourPackageName = b.TourPackageName,
                            Date = b.Date.ToString("yyyy-MM-dd")
                        })
                        .OrderBy(b => b.Date)
                        .ToArray()
                })
                .ToArray();


            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }
    }
}
