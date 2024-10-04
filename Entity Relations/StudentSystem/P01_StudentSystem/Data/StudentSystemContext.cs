using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public const string ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=StudentSystem;Trusted_Connection=True;";

        public StudentSystemContext(DbContextOptions options)
            : base(options)
        {

        }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<Homework> Homeworks { get; set; }
        public virtual DbSet<StudentCourse> StudentsCourses { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(ConnectionString);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
