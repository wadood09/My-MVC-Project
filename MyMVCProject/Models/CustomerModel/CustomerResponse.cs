using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.CustomerModel
{
    public class CustomerResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public string? ImageUrl { get; set; }
    }
}
