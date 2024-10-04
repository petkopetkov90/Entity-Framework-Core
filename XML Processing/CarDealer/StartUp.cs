using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            using var context = new CarDealerContext();

            //var inputXml = File.ReadAllText("../../../Datasets/sales.xml");

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        //9
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<SupplierImportDto>), new XmlRootAttribute("Suppliers"));

            using var reader = new StringReader(inputXml);

            var suppliersDto = (List<SupplierImportDto>)xmlSerializer.Deserialize(reader);

            var suppliers = suppliersDto
                .Where(dto => dto.Name != null)
                .Select(dto => new Supplier()
                {
                    Name = dto.Name,
                    IsImporter = dto.IsImporter
                })
                .ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        //10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<PartImportDto>), new XmlRootAttribute("Parts"));

            using var reader = new StringReader(inputXml);

            var partsDto = (List<PartImportDto>)xmlSerializer.Deserialize(reader);

            var supplierIds = context.Suppliers
                .Select(s => s.Id)
                .ToList();

            var parts = partsDto
                .Where(dto => supplierIds.Contains(dto.SupplierId))
                .Select(dto => new Part()
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Quantity = dto.Quantity,
                    SupplierId = dto.SupplierId
                })
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        //11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<CarImportDto>), new XmlRootAttribute("Cars"));

            using var reader = new StringReader(inputXml);

            var carsDto = (List<CarImportDto>)xmlSerializer.Deserialize(reader);

            List<Car> cars = new List<Car>();

            foreach (var carDto in carsDto)
            {
                Car car = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TraveledDistance = carDto.TraveledDistance
                };

                List<int> partsIds = carDto.PartsIds.Select(p => p.Id).Distinct().ToList();

                var partsCars = new List<PartCar>();

                foreach (var partId in partsIds)
                {
                    partsCars.Add(new PartCar()
                    {
                        Car = car,
                        PartId = partId
                    });
                }

                car.PartsCars = partsCars;
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<CustomerImportDto>), new XmlRootAttribute("Customers"));

            using var reader = new StringReader(inputXml);

            var customersDto = (List<CustomerImportDto>)xmlSerializer.Deserialize(reader);

            var customers = customersDto
                .Select(dto => new Customer()
                {
                    Name = dto.Name,
                    BirthDate = dto.BirthDate,
                    IsYoungDriver = dto.IsYoungDriver
                })
                .ToList();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        //13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<SaleImportDto>), new XmlRootAttribute("Sales"));

            using var reader = new StringReader(inputXml);

            var salesDto = (List<SaleImportDto>)xmlSerializer.Deserialize(reader);

            var cars = context.Cars
                .Select(c => c.Id)
                .ToList();

            var sales = salesDto
                .Where(dto => cars.Contains(dto.CarId))
                .Select(dto => new Sale()
                {
                    CarId = dto.CarId,
                    CustomerId = dto.CustomerId,
                    Discount = dto.Discount
                })
                .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        //14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .Select(c => new CarExportDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .ToList();

            return SerializeToXml(cars, "cars");
        }

        //15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new CarBmwExportDto()
                {
                    Id = c.Id,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .ToList();

            return SerializeToXml(cars, "cars");
        }

        //16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new LocalSupplierExportDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();


            return SerializeToXml(suppliers, "suppliers");
        }

        //17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new CarWithPartsExportDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                    Parts = c.PartsCars
                        .Select(pc => new PartExportDto()
                        {
                            Name = pc.Part.Name,
                            Price = pc.Part.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToList()
                })
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToList();

            return SerializeToXml(cars, "cars");
        }

        //18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new CustomerExportDto
                {
                    FullName = c.Name,
                    CarsBought = c.Sales.Count(),
                    MoneySpent = c.Sales.Sum(s =>
                        s.Car.PartsCars.Sum(pc =>
                            Math.Round(c.IsYoungDriver ? pc.Part.Price * 0.95m : pc.Part.Price, 2)
                            )
                        )

                })
                .OrderByDescending(c => c.MoneySpent)
                .ToList();

            return SerializeToXml(customers, "customers");
        }

        //19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new SaleExportDto()
                {
                    Car = new CarExportAttributeDto()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    Discount = (int)s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = Math.Round(s.Car.PartsCars.Sum(pc => pc.Part.Price), 4),
                    PriceWithDiscount = Math.Round((double)(s.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - (s.Discount / 100))), 4)
                })
                .ToList();

            return SerializeToXml(sales, "sales");
        }


        private static string SerializeToXml<T>(T dto, string rootAttribute)
        {
            var xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootAttribute));

            var sb = new StringBuilder();

            using var writer = new StringWriter(sb, CultureInfo.InvariantCulture);

            var xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(writer, dto, xmlSerializerNamespaces);

            return sb.ToString();
        }
    }
}