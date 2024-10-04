using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using TravelAgency.Data;
using TravelAgency.Data.Models;
using TravelAgency.DataProcessor.ImportDtos;

namespace TravelAgency.DataProcessor
{
    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data format!";
        private const string DuplicationDataMessage = "Error! Data duplicated.";
        private const string SuccessfullyImportedCustomer = "Successfully imported customer - {0}";
        private const string SuccessfullyImportedBooking = "Successfully imported booking. TourPackage: {0}, Date: {1}";

        public static string ImportCustomers(TravelAgencyContext context, string xmlString)
        {
            var customersDto = XmlSerializationHelper.Deserialize<List<CustomerImportDto>>(xmlString, "Customers");

            StringBuilder stringBuilder = new StringBuilder();

            var customers = new List<Customer>();

            foreach (var dto in customersDto)
            {
                if (!IsValid(dto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                if (customers.Any(c => c.FullName == dto.FullName
                                       || c.Email == dto.Email
                                       || c.PhoneNumber == dto.PhoneNumber))
                {
                    stringBuilder.AppendLine(DuplicationDataMessage);
                    continue;
                }

                Customer customer = new Customer()
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber
                };

                customers.Add(customer);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedCustomer, customer.FullName));
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportBookings(TravelAgencyContext context, string jsonString)
        {
            var settings = new JsonSerializerSettings()
            {
                Culture = CultureInfo.InvariantCulture
            };

            var bookingsDto = (List<BookingImportDto>)JsonConvert.DeserializeObject(jsonString, typeof(List<BookingImportDto>), settings);

            StringBuilder stringBuilder = new StringBuilder();

            var bookings = new List<Booking>();

            foreach (var dto in bookingsDto)
            {
                if (!IsValid(dto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                if (!IsDateInFormat(dto.BookingDate, "yyyy-MM-dd"))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Booking booking = new Booking()
                {
                    BookingDate = DateTime.ParseExact(dto.BookingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Customer = context.Customers.First(c => c.FullName == dto.CustomerName),
                    TourPackage = context.TourPackages.First(t => t.PackageName == dto.TourPackageName)
                };

                bookings.Add(booking);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedBooking, dto.TourPackageName,
                    booking.BookingDate.ToString("yyyy-MM-dd")));
            }

            context.Bookings.AddRange(bookings);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }
        public static bool IsDateInFormat(string date, string format)
        {

            return DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }


        public static bool IsValid(object dto)
        {
            var validateContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validateContext, validationResults, true);

            foreach (var validationResult in validationResults)
            {
                string currValidationMessage = validationResult.ErrorMessage;
            }

            return isValid;
        }
    }
}
