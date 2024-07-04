using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.Models
{
    public class RolesModel
    {
        [Key]
        [Required]
        [Display(Name = "Id")]
        public Int32 id { get; set; }
        
        [Required]
        [Display(Name = "Role Name")]
        public string? role_name { get; set; }

        [Required]
        [Display(Name = "Role Type")]
        public string? role_type { get; set; }

        [Required]
        [Display(Name = "Remarks")]
        public string? remarks { get; set; }

        [Display(Name = "IsAdmin")]
        public bool is_sys_admin { get; set; }

        public virtual Int32 created_by { get; set; }
    }
}