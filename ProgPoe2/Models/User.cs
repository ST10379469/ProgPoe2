using System.ComponentModel.DataAnnotations;

namespace ProgPoe2.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "HR", "Lecturer", "Coordinator", "Manager"

        public decimal HourlyRate { get; set; } = 650m;

        public bool IsActive { get; set; } = true;

        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}

