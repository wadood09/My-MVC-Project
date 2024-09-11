using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Core.Domain.Entities
{
    public class User : Auditables
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public string? ImageUrl { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; } = default!;
    }
}
