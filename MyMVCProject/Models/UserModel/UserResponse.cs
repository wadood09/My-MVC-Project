using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.UserModel
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public string? ImageUrl { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = default!;
    }
}
