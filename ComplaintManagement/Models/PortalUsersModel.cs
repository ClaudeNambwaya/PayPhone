using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.Models
{
    public class PortalUsersModel
    {
        public PortalUsersModel()
        {
            roles = new HashSet<RolesModel>();
        }

        [Key]
        [Display(Name = "Id")]
        public Int32 id { get; set; }

        [Display(Name = "Role"), Required(ErrorMessage = "Value is required.")]
        public Int32 role_id { get; set; }

        [Display(Name = "Mobile"), Required(ErrorMessage = "Value is required.")]
        public string mobile { get; set; }

        [Display(Name = "Email"), Required(ErrorMessage = "Value is required.")]
        public string email { get; set; }

        [Display(Name = "Name"), Required(ErrorMessage = "Value is required.")]
        public string name { get; set; }

        public string password { get; set; }

        [Display(Name = "Avatar")]
        public virtual string avatar { get; set; }

        [Display(Name = "Locked")]
        public bool locked { get; set; }

        [Display(Name = "Google Authenticate")]
        public bool google_authenticate { get; set; }

        [Display(Name = "Google Security Key")]
        public virtual string sec_key { get; set; }

        public virtual Int32 created_by { get; set; }

        public virtual ICollection<RolesModel> roles { get; set; }

    }
}