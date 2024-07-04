namespace PayPhone.Dtos
{
    public class RegistrationUserDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Mobile { get; set; }
        public string? RoleName { get; set; }
        public string? Password { get; set; }
        public bool Approved { get; set; }
        public bool Locked { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
