using MiniORM;
using MiniORMTest.Data.Entities;

namespace MiniORMTest;

public class SoftUniDbContextClass : DbContext
{
    public SoftUniDbContextClass(string connectionString)
        : base(connectionString)
    {
    }

    public DbSet<Employee> Employees { get; }
    public DbSet<Department> Departments { get; }
    public DbSet<Project> Projects { get; }
    public DbSet<EmployeesProject> EmployeesProjects { get; }
}

