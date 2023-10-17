using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string? email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? password { get; set; }

        public string? ReturnURL { get; set; }

        public bool isRemember { get; set; }

    }
}