using Microsoft.EntityFrameworkCore;
using SchoolMenuPlanner.Models;
using System.Windows;

namespace SchoolMenuPlanner.Data
{
    public class MenuPlannerContext : DbContext
    {
        public DbSet<Dish> Dishes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Application.Current.FindResource("ConnectionString") as string;

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = @"Data Source=DANKOR22;Initial Catalog=SchoolMenuPlanner;Integrated Security=True;TrustServerCertificate=True";
            }

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Явно указываем имя таблицы и столбцов
            modelBuilder.Entity<Dish>().ToTable("dishes");
            modelBuilder.Entity<Dish>().HasKey(d => d.Id);
            modelBuilder.Entity<Dish>().Property(d => d.Id).HasColumnName("id");
            modelBuilder.Entity<Dish>().Property(d => d.Name).HasColumnName("name");

            // УБЕРИТЕ все упоминания meal_type
        }
    }
}