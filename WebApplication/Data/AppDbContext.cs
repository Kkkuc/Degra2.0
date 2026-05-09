using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Log> Logs { get; set; }
        
        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<FieldOfStudy> FieldsOfStudy { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<ScheduleChange> ScheduleChanges { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Group>()
                .HasOne(g => g.FieldOfStudy)
                .WithMany(f => f.Groups)
                .HasForeignKey(g => g.FieldOfStudyId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Group>()
                .HasOne(g => g.Specialization)
                .WithMany(s => s.Groups)
                .HasForeignKey(g => g.SpecializationId)
                .IsRequired(false) 
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StudentGroup>()
                .HasKey(sg => new { sg.StudentId, sg.GroupId });

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });
            
            
             modelBuilder.Entity<Timetable>()
                .Property(t => t.WeekCycle)
                .HasConversion<string>();

            modelBuilder.Entity<Timetable>()
                .Property(t => t.ClassType)
                .HasConversion<string>();

            modelBuilder.Entity<Group>()
                .Property(g => g.ClassType)
                .HasConversion<string>();

            modelBuilder.Entity<Room>()
                .Property(r => r.RoomType)
                .HasConversion<string>();
             
        }
    }
}