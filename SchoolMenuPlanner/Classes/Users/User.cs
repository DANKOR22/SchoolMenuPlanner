using System.ComponentModel.DataAnnotations;

namespace SchoolMenuPlanner.Classes
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string login { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string password { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string full_name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string role { get; set; } = string.Empty;
    }
}