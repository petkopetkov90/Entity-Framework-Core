using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new ProductShopContext();

            //var jsonString = File.ReadAllText("../../../Datasets/categories-products.json");

            //Console.WriteLine(ImportCategoryProducts(context, jsonString));

            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {

            var users = JsonConvert.DeserializeObject<List<User>>(inputJson);

            context.Users.AddRange(users);

            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {

            var products = JsonConvert.DeserializeObject<List<Product>>(inputJson);

            context.Products.AddRange(products);

            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<List<Category>>(inputJson).Where(p => p.Name != null);

            context.Categories.AddRange(categories);

            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoriesProducts = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);

            context.CategoriesProducts.AddRange(categoriesProducts);

            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count()}";
        }

        private static JsonSerializerSettings JsonSerializerSettings()
        {
            var jsonSetting = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            return jsonSetting;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new
                {
                    Name = p.Name,
                    Price = p.Price,
                    Seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                })
                .OrderBy(p => p.Price)
                .ToList();

            var jsonProducts = JsonConvert.SerializeObject(products, JsonSerializerSettings());

            return jsonProducts;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Where(sp => sp.Buyer != null)
                        .Select(sp => new
                        {
                            Name = sp.Name,
                            Price = sp.Price,
                            BuyerFirstName = sp.Buyer!.FirstName,
                            BuyerLastName = sp.Buyer.LastName
                        }).ToList()
                }).ToList();

            var jsonUsers = JsonConvert.SerializeObject(users, JsonSerializerSettings());

            return jsonUsers;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count)
                .Select(c => new
                {
                    Category = c.Name,
                    ProductsCount = c.CategoriesProducts.Count,
                    AveragePrice = $"{c.CategoriesProducts.Average(cp => cp.Product.Price):f2}",
                    TotalRevenue = $"{c.CategoriesProducts.Sum(cp => cp.Product.Price):f2}"
                }).ToList();

            var jsonCategories = JsonConvert.SerializeObject(categories, JsonSerializerSettings());

            return jsonCategories;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersProducts = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = u.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new
                        {
                            Name = p.Name,
                            Price = p.Price,
                        }).ToList()
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .ToList();

            var users = new
            {
                UsersCount = usersProducts.Count,
                Users = usersProducts.Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    SoldProducts = new
                    {
                        Count = u.SoldProducts.Count,
                        Products = u.SoldProducts
                                .Select(p => new
                                {
                                    p.Name,
                                    p.Price
                                })
                    }

                })
            };

            var jsonCategories = JsonConvert.SerializeObject(users, JsonSerializerSettings());

            return jsonCategories;
        }

    }
}