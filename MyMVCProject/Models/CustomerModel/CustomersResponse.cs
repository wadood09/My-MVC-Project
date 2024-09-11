using MyMVCProject.Core.Domain.Enums;

namespace MyMVCProject.Models.CustomerModel
{
    public class CustomersResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public int NoOfOrdersMade { get; set; }
    }
}
