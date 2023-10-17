using ComplaintManagement.Models;
using System.Data;

public class RBACUser
{
    public DBHandler dbhandler { get; set; }
    public int User_Id { get; set; }
    public bool IsSysAdmin { get; set; }
    public string Username { get; set; }

    private List<UserRole> Roles = new List<UserRole>();

    public RBACUser(string _username) 
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        IConfiguration configuration = builder.Build();

        this.Username = _username;
        this.IsSysAdmin = false;
        GetDatabaseUserRolesPermissions(configuration);
    }

    private void GetDatabaseUserRolesPermissions(IConfiguration configuration)
    {
        dbhandler = new DBHandler(configuration["ConnectionStrings:DefaultConnection"]);
        DataTable user_dt = new DataTable();
        user_dt = dbhandler.ValidateUserLogin("ADMIN", this.Username);
        if (user_dt.Rows.Count > 0)
        {
            foreach (DataRow user_dr in user_dt.Rows)
            {
                PortalUsersModel _user = new PortalUsersModel
                {
                    id = Convert.ToInt32(user_dr["id"])!,
                    name = Convert.ToString(user_dr["name"])!,
                    email = Convert.ToString(user_dr["email"])!
                };

                if (_user != null)
                {
                    this.User_Id = _user.id;

                    //get roles associated with the user
                    DataTable user_roles_dt = dbhandler.GetRecordsById("user_allocated_roles", Convert.ToInt32(user_dr["id"]));

                    if (user_roles_dt.Rows.Count > 0)
                    {
                        foreach (DataRow user_roles_dr in user_roles_dt.Rows)
                        {
                            UserRole _userRole = new UserRole { Role_Id = Convert.ToInt16(user_roles_dr["role_id"]), RoleName = Convert.ToString("role_name") };

                            //get role permissions
                            DataTable role_permissions_dt = new DataTable();
                            role_permissions_dt = dbhandler.GetRecordsById("role_allocated_permissions", Convert.ToInt16(user_roles_dr["role_id"]));
                            foreach (DataRow role_permissions_dr in role_permissions_dt.Rows)
                            {
                                _userRole.Permissions.Add(new RolePermission { Permission_Id = Convert.ToInt16(role_permissions_dr["permission_id"]), PermissionDescription = Convert.ToString(role_permissions_dr["permission_name"]) });
                            }
                            this.Roles.Add(_userRole);

                            if (!this.IsSysAdmin)
                                this.IsSysAdmin = Convert.ToBoolean(user_roles_dr["is_sys_admin"]);
                        }
                    }
                }
            }
        }
    }

    public bool HasPermission(string requiredPermission)
    {
        bool bFound = false;
        foreach (UserRole role in this.Roles)
        {
            bFound = (role.Permissions.Where(p => p.PermissionDescription.ToLower() == requiredPermission.ToLower()).ToList().Count > 0);
            if (bFound)
                break;
        }
        return bFound;
    }

    public bool HasRole(string role)
    {
        return (Roles.Where(p => p.RoleName == role).ToList().Count > 0);
    }

    public bool HasRoles(string roles)
    {
        bool bFound = false;
        string[] _roles = roles.ToLower().Split(';');
        foreach (UserRole role in this.Roles)
        {
            try
            {
                bFound = _roles.Contains(role.RoleName.ToLower());
                if (bFound)
                    return bFound;
            }
            catch (Exception)
            {
            }
        }
        return bFound;
    }
}

public class UserRole
{
    public int Role_Id { get; set; }
    public string RoleName { get; set; }

    public List<RolePermission> Permissions = new List<RolePermission>();
}

public class RolePermission
{
    public int Permission_Id { get; set; }
    public string PermissionDescription { get; set; }
}