using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.CustomerModel
{
    public class CustomerRequest
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public IFormFile? Image { get; set; }
    }
}
