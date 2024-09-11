using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.UserModel
{
    public class UsersResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string Role { get; set; } = default!;
    }
}
