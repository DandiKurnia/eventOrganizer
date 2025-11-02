using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class UserModel
    {
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Name wajib diisi")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Customer"; // default

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
