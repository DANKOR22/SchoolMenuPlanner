using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace SchoolMenuPlanner.Classes
{
    public class Dish
    {
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("course_type_id")]
        public int CourseTypeId { get; set; }

        [ForeignKey("CourseTypeId")]
        public CourseType? CourseType { get; set; }
    }

    public class CourseType
    {
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    }
}