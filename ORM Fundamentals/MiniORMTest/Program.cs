namespace MiniORMTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=MiniORM;Integrated Security=True;TrustServerCertificate=true";

            var dbContext = new SoftUniDbContextClass(connectionString);

            //dbContext.Employees.Add(new Employee()
            //{
            //    FirstName = "Gosho",
            //    LastName = "Inserted",
            //    DepartmentId = dbContext.Departments.First().Id,
            //    IsEmployed = true
            //});

            var employee = dbContext.Employees.Last();

            employee.FirstName = "modified";

            dbContext.SaveChanges();
        }
    }
}
