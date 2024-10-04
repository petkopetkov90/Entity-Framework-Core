using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new CarDealerContext();

            //var inputJson = File.ReadAllText("../../../Datasets/sales.json");

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.Suppliers.AddRange(suppliers);

            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson)
                .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId)).ToList();

            context.Parts.AddRange(parts);

            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsDto = JsonConvert.DeserializeObject<List<CarDto>>(inputJson);

            var cars = new List<Car>();
            var partsCars = new List<PartCar>();

            foreach (var carDto in carsDto)
            {
                var car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TraveledDistance = carDto.TraveledDistance
                };

                cars.Add(car);

                foreach (var part in carDto.PartsId.Distinct())
                {
                    var partCar = new PartCar
                    {
                        Car = car,
                        PartId = part
                    };

                    partsCars.Add(partCar);
                }
            }

            context.Cars.AddRange(cars);
            context.PartsCars.AddRange(partsCars);

            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customers);

            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(sales);

            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate,
                    c.IsYoungDriver
                })
                .ToList();

            var jsonCustomers = JsonConvert.SerializeObject(customers, JsonSerializerSettings());

            return jsonCustomers;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TraveledDistance

                }).ToList();

            var jsonCustomers = JsonConvert.SerializeObject(cars, JsonSerializerSettings());

            return jsonCustomers;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count

                }).ToList();

            var jsonCustomers = JsonConvert.SerializeObject(suppliers, JsonSerializerSettings());

            return jsonCustomers;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithPars = context.Cars
                .Include(c => c.PartsCars)
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TraveledDistance,
                    },

                    parts = c.PartsCars.Select(cp => new
                    {
                        Name = cp.Part.Name,
                        Price = $"{cp.Part.Price:f2}"
                    })

                }).ToList();

            var jsonCarParts = JsonConvert.SerializeObject(carsWithPars, JsonSerializerSettings());

            return jsonCarParts;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Sum(s => s.Car.PartsCars.Sum(p => p.Part.Price))

                })
                .OrderByDescending(c => c.SpentMoney)
                .ThenByDescending(c => c.BoughtCars)
                .ToList();

            var jsonCarParts = JsonConvert.SerializeObject(customers, JsonSerializerSettings());

            return jsonCarParts;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    customerName = s.Customer.Name,
                    discount = $"{s.Discount:f2}",
                    price = $"{s.Car.PartsCars.Sum(p => p.Part.Price):f2}",
                    priceWithDiscount = $"{(s.Car.PartsCars.Sum(p => p.Part.Price) * (1 - (s.Discount / 100))):f2}"
                })
                .ToList();

            var jsonCarParts = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return jsonCarParts;
        }

        private static JsonSerializerSettings JsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return settings;
        }
    }
}