/*using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace TimetableApp.Data
{
    public class TimetableDbContext(DbContextOptions<TimetableDbContext> options) : DbContext(options)
    {
        // Organizational Structure
        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<FieldOfStudy> FieldsOfStudy { get; set; }
        public DbSet<Specialization> Specializations { get; set; }

        // Infrastructure & People
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }

        // Academics
        public DbSet<ClassType> ClassTypes { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }

        // Schedule & Logistics
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<ScheduleChange> ScheduleChanges { get; set; }

        // RBAC & Auth
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite Key for StudentGroup (Many-to-Many)
            modelBuilder.Entity<StudentGroup>()
                .HasKey(sg => new { sg.StudentId, sg.GroupId });

            // Composite Key for RolePermission (Many-to-Many)
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Configure Timetable Relationships (optional: explicit delete behaviors)
            modelBuilder.Entity<Timetable>()
                .HasOne(t => t.Group)
                .WithMany()
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique index for Student ID and Login
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentID)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}*/