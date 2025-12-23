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

        [Required]
        [RegularExpression("^[1-9][0-9]*$",
            ErrorMessage = "Nomor HP tidak boleh diawali angka 0")]
        public string PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "Password wajib diisi")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Customer";

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class EditUserViewModel
    {
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Name wajib diisi")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[1-9][0-9]*$",
            ErrorMessage = "Nomor HP tidak boleh diawali angka 0")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }

}
