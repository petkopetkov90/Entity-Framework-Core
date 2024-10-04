using SoftUni.Data;
using SoftUni.Models;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main()
        {
            var context = new SoftUniContext();

            Console.WriteLine(RemoveTown(context));
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    Id = e.EmployeeId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    MiddleName = e.MiddleName,
                    JobTitle = e.JobTitle,
                    Salary = e.Salary
                })
                .OrderBy(e => e.Id)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .Select(e => $"{e.FirstName} - {e.Salary:f2}")
                .ToList();

            return string.Join(Environment.NewLine, employees);
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => $"{e.FirstName} {e.LastName} from {e.Department.Name} - ${e.Salary:f2}")
                .ToList();

            return string.Join(Environment.NewLine, employees).TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)

        {
            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            Employee employee = context.Employees.FirstOrDefault(e => e.LastName.Equals("Nakov"));

            if (employee is null)
            {
                throw new ArgumentNullException("The employee does not exist!");
            }

            employee.Address = address;

            context.SaveChanges();

            var employees = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address.AddressText)
                .ToList();

            return string.Join(Environment.NewLine, employees).TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Take(10)
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    ManagerFirstName = e.Manager.FirstName != null ? e.Manager.FirstName : "",
                    ManagerLastName = e.Manager.LastName != null ? e.Manager.LastName : "",
                    Projects = e.EmployeesProjects
                        .Where(ep => ep.Project.StartDate.Year > 2000 && ep.Project.StartDate.Year < 2004)
                        .Select(ep => new
                        {
                            ProjectName = ep.Project.Name,
                            StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                            EndDate = ep.Project.EndDate.HasValue
                                ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished"
                        })
                        .ToList()
                }).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

                foreach (var p in e.Projects)
                {
                    sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .Select(a => $"{a.AddressText}, {a.Town.Name} - {a.Employees.Count} employees")
                .ToList();

            return string.Join(Environment.NewLine, addresses).TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees.Find(147);

            var projects = context.EmployeesProjects
                .Where(ep => ep.EmployeeId == 147)
                .Select(ep => new
                {
                    ep.Project.Name
                })
                .OrderBy(ep => ep.Name)
                .ToList();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var ep in projects)
            {
                sb.AppendLine(ep.Name);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    Department = $@"{d.Name} - {d.Manager.FirstName} {d.Manager.LastName}",
                    Employees = d.Employees
                        .OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName)
                        .Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle}")
                        .ToList()

                })
                .ToList();


            StringBuilder sb = new StringBuilder();

            foreach (var d in departments)
            {
                sb.AppendLine(d.Department);

                foreach (var e in d.Employees)
                {
                    sb.AppendLine(e);
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .Select(p => $"{p.Name}{Environment.NewLine}{p.Description}{Environment.NewLine}{p.StartDate:M/d/yyyy h:mm:ss tt}")
                .ToList();

            return string.Join(Environment.NewLine, projects);
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            string[] departments = new string[]
            {
                "Engineering",
                "Tool Design",
                "Marketing",
                "Information Services"
            };

            var employees = context.Employees
                .Where(e => departments.Contains(e.Department.Name))
                .ToList();

            foreach (var e in employees)
            {
                e.Salary += e.Salary * 0.12m;
            }

            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.FirstName.ToLower().StartsWith("sa"))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})")
                .ToList();

            return string.Join(Environment.NewLine, employees);
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            context.EmployeesProjects.RemoveRange(context.EmployeesProjects.Where(ep => ep.ProjectId == 2));

            context.Projects.Remove(context.Projects.Find(2));

            context.SaveChanges();

            var projects = context.Projects
                .Take(10)
                .Select(p => p.Name)
                .ToList();

            return string.Join(Environment.NewLine, projects);
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns.FirstOrDefault(t => t.Name.Equals("Seattle"));

            var addresses = new List<Address>();

            if (town is not null)
            {
                addresses = context.Addresses.Where(a => a.TownId == town.TownId).ToList();


                var employees = context.Employees.Where(e => addresses.Contains(e.Address))
                    .ToList();

                foreach (var e in employees)
                {
                    e.AddressId = null;
                }

                context.SaveChanges();

                if (addresses.Any())
                {
                    context.Addresses.RemoveRange(addresses);
                }

                context.SaveChanges();

                if (town is not null)
                {
                    context.Towns.Remove(town);
                }

                context.SaveChanges();

                return $"{addresses.Count} addresses in {town.Name} were deleted";
            }

            return $"This town does not exist";
        }
    }
}
