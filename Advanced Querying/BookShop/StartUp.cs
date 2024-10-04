using BookShop.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace BookShop
{
    using Data;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            Console.WriteLine(GetBooksReleasedBefore(db, "12-04-1992"));

        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            Enum.TryParse(command, true, out AgeRestriction restriction);

            var booksByAgeRestriction = context.Books
                .Where(b => b.AgeRestriction == restriction)
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            return string.Join(Environment.NewLine, booksByAgeRestriction);
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, goldenBooks);
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var booksByPrice = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => $"{b.Title} - ${b.Price:f2}")
                .ToList();

            return string.Join(Environment.NewLine, booksByPrice);
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var booksByReleaseYear = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, booksByReleaseYear);
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            List<string> categories = input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

            var booksByCategory = context.Categories
                .Where(c => categories.Contains(c.Name.ToLower()))
                .Include(c => c.CategoryBooks)
                .ThenInclude(cb => cb.Book)
                .SelectMany(c => c.CategoryBooks)
                .Select(cb => cb.Book.Title)
                .OrderBy(t => t)
                .ToList();

            return string.Join(Environment.NewLine, booksByCategory);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime currentDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var bookByReleaseDate = context.Books
                .Where(b => b.ReleaseDate != null && b.ReleaseDate.Value < currentDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}")
                .ToList();

            return string.Join(Environment.NewLine, bookByReleaseDate).TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authorsByEndOfName = context.Authors
                .Where(a => a.FirstName.ToLower().EndsWith(input.ToLower()))
                .Select(a => $"{a.FirstName} {a.LastName}")
                .OrderBy(n => n)
                .ToList();

            return string.Join(Environment.NewLine, authorsByEndOfName).TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var bookTitlesContainsString = context.Books
                .Select(b => b.Title)
                .Where(t => t.ToLower().Contains(input.ToLower()))
                .OrderBy(t => t)
                .ToList();

            return string.Join(Environment.NewLine, bookTitlesContainsString);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var booksByAuthorLastNameStartsWith = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})")
                .ToList();

            return string.Join(Environment.NewLine, booksByAuthorLastNameStartsWith);
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                .Count(b => b.Title.Length > lengthCheck);
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authorsByBooksCopies = context.Authors
                .OrderByDescending(a => a.Books.Sum(b => b.Copies))
                .Select(a => $"{a.FirstName} {a.LastName} - {a.Books.Sum(b => b.Copies)}")
                .ToList();

            return string.Join(Environment.NewLine, authorsByBooksCopies);
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var profitByCategory = context.Categories
                .Include(c => c.CategoryBooks)
                .ThenInclude(cb => cb.Book)
                .Select(c => new
                {
                    Category = c.Name,
                    CategoryProfit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)

                })
                .OrderByDescending(c => c.CategoryProfit)
                .ThenBy(c => c.Category)
                .Select(c => $"{c.Category} ${c.CategoryProfit:f2}")
                .ToArray();

            return string.Join(Environment.NewLine, profitByCategory);
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categoriesMostRecentBooks = context.Categories
                .Include(c => c.CategoryBooks)
                .ThenInclude(cb => cb.Book)
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Books = c.CategoryBooks.
                        OrderByDescending(b => b.Book.ReleaseDate)
                        .Take(3)
                        .Select(cb => new
                        {
                            BookTitle = cb.Book.Title,
                            ReleaseYear = cb.Book.ReleaseDate!.Value.Year
                        })
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var category in categoriesMostRecentBooks)
            {
                sb.AppendLine($"--{category.CategoryName}");

                foreach (var book in category.Books)
                {
                    sb.AppendLine($"{book.BookTitle} ({book.ReleaseYear})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books.Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year < 2010);

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books.Where(b => b.Copies < 4200).ToList();

            context.Books.RemoveRange(books);

            context.SaveChanges();

            return books.Count;
        }

    }
}


