using Microsoft.EntityFrameworkCore;
using SchoolMenuPlanner.Models;

namespace SchoolMenuPlanner.Data
{
    public class PlannerContext : DbContext
    {
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<CourseType> CourseTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DANKOR22;Initial Catalog=SchoolMenuPlanner;Integrated Security=True;Trust Server Certificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка таблицы Dish
            modelBuilder.Entity<Dish>(entity =>
            {
                entity.ToTable("dishes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.CourseTypeId).HasColumnName("course_type_id");

                entity.HasOne(d => d.CourseType)
                    .WithMany(ct => ct.Dishes)
                    .HasForeignKey(d => d.CourseTypeId);
            });

            // Настройка таблицы CourseType
            modelBuilder.Entity<CourseType>(entity =>
            {
                entity.ToTable("course_types");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
            });
        }
    }
}