using SchoolMenuPlanner.Classes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace SchoolMenuPlanner.Models
{
    public class DailyMenu
    {
        public int Id { get; set; }

        [Column("day_of_week")]
        public int DayOfWeek { get; set; } // 1-Понедельник, 2-Вторник и т.д.

        [Column("meal_type")]
        public string MealType { get; set; } = string.Empty; // "Breakfast" или "Lunch"

        [Column("dish_id")]
        public int DishId { get; set; }

        [Column("category_type")]
        public string CategoryType { get; set; } = string.Empty; // "HotDish", "Drink", "Fruit" и т.д.

        [Column("week_number")]
        public int WeekNumber { get; set; }

        [Column("year")]
        public int Year { get; set; }

        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }
    }
}