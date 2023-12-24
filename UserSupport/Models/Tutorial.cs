using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserSupport.Models
{
    public class Tutorial
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string? CoverImage { get; set; }
        public DateTime? CreatedAt { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; } = 1; // fk user
        public User? User { get; set; }
        public string? Content { get; set; }
    }
}

