using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using var context = new ProductShopContext();

            //string inputXml = File.ReadAllText("../../../Datasets/categories-products.xml");

            Console.WriteLine(GetUsersWithProducts(context));
        }

        //1
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(UserImportDto[]), new XmlRootAttribute("Users"));

            using var reader = new StringReader(inputXml);

            var usersDto = (UserImportDto[])xmlSerializer.Deserialize(reader);

            List<User> users = usersDto
                .Select(dto => new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Age = dto.Age,
                })
                .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        //2
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<ProductImportDto>), new XmlRootAttribute("Products"));

            using var reader = new StringReader(inputXml);

            var productsDto = (List<ProductImportDto>)xmlSerializer.Deserialize(reader);

            var products = productsDto
                .Select(dto => new Product
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    SellerId = dto.SellerId,
                    BuyerId = dto.BuyerId
                })
                .ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //3
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<CategoryImportDto>), new XmlRootAttribute("Categories"));

            using var reader = new StringReader(inputXml);

            var categoriesDto = (List<CategoryImportDto>)xmlSerializer.Deserialize(reader);

            var categories = categoriesDto
                .Select(dto => new Category
                {
                    Name = dto.Name
                })
                .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        //4
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<CategoryProductImportDto>), new XmlRootAttribute("CategoryProducts"));

            using var reader = new StringReader(inputXml);

            var categoriesProductsDto = (List<CategoryProductImportDto>)xmlSerializer.Deserialize(reader);

            var categoriesProducts = categoriesProductsDto
                .Where(cp => cp is { CategoryId: not null, ProductId: not null })
                .Select(dto => new CategoryProduct
                {
                    ProductId = (int)dto.ProductId,
                    CategoryId = (int)dto.CategoryId
                })
                .ToList();

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count}";
        }

        //5
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ProductInPriceRangeExportDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = (p.Buyer != null) ? $"{p.Buyer.FirstName} {p.Buyer.LastName}" : " "
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToList();

            return SerializeToXml(products, "Products");
        }

        //6
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UserWithSoldItemExportDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ProductsSold = u.ProductsSold
                        .Select(p => new ProductSoldExportDto()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .ToList()
                })
                .Take(5)
                .ToList();

            return SerializeToXml(users, "Users");
        }

        //7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new CategoryExportDto()
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToList();

            return SerializeToXml(categories, "Categories");
        }

        //8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderByDescending(u => u.ProductsSold.Count)
                .Select(u => new UserAndProductExportDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new ProductSoldWithCountExportDto()
                    {
                        Count = u.ProductsSold.Count,
                        SoldProducts = u.ProductsSold
                            .Select(p => new ProductSoldExportDto()
                            {
                                Name = p.Name,
                                Price = p.Price
                            })
                            .OrderByDescending(p => p.Price)
                            .ToList()
                    }
                })
                .ToList();

            var output = new UsersWithCountExportDto
            {
                Count = users.Count,
                UserAndProducts = users.Take(10).ToList()
            };

            return SerializeToXml(output, "Users");
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