using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.CustomerModel
{
    public class UpdateCustomerRequest
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? Image { get; set; }
    }
}
