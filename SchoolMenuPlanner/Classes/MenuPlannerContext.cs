using Microsoft.EntityFrameworkCore;
using SchoolMenuPlanner.Models;
using System.Windows.Media;

namespace SchoolMenuPlanner.Classes
{
    public class PlannerContext : DbContext
    {
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<CourseType> CourseTypes { get; set; }
        public DbSet<DailyMenu> DailyMenus { get; set; }
        public DbSet<User> Users { get; set; }

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

            // Настройка таблицы DailyMenu
            modelBuilder.Entity<DailyMenu>(entity =>
            {
                entity.ToTable("daily_menus");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DayOfWeek).HasColumnName("day_of_week");
                entity.Property(e => e.MealType).HasColumnName("meal_type");
                entity.Property(e => e.DishId).HasColumnName("dish_id");
                entity.Property(e => e.CategoryType).HasColumnName("category_type");
                entity.Property(e => e.WeekNumber).HasColumnName("week_number");
                entity.Property(e => e.Year).HasColumnName("year");

                entity.HasOne(d => d.Dish)
                    .WithMany()
                    .HasForeignKey(d => d.DishId);
            });
            
        }
    }
}