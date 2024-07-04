using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.Models
{
    public class PermissionsModel
    {
        [Key]
        [Required]
        [Display(Name = "Permission Id")]
        public Int32 id { get; set; }
        
        [Required]
        [Display(Name = "Permission Name")]
        public string? permission_name { get; set; }

        public virtual Int32 created_by { get; set; }
    }
}