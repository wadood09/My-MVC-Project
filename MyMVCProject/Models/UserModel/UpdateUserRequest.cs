using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.UserModel
{
    public class UpdateUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public int? RoleId { get; set; }
    }
}
