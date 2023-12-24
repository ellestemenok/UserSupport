using System.ComponentModel.DataAnnotations;
namespace UserSupport.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Role { get; set; } = "Читатель";
        public string? ProfileImage { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Position { get; set; }
        public bool IsVerified { get; set; } = false;
        public List<Tutorial>? Tutorials { get; set; }
    }
}
