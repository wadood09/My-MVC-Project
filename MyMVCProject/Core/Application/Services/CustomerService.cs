using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Domain.Entities;
using MyMVCProject.Models;
using MyMVCProject.Models.CustomerModel;
using System.Security.Claims;

namespace MyMVCProject.Core.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletRepository _walletRepository;
        private readonly IHttpContextAccessor _httpContext;

        public CustomerService(ICustomerRepository customerRepository, IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContext, IFileRepository fileRepository, IWalletRepository walletRepository)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
            _fileRepository = fileRepository;
            _walletRepository = walletRepository;
        }

        public async Task<BaseResponse> CreateCustomer(CustomerRequest request)
        {
            var exists = await _userRepository.ExistsAsync(request.Email);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = "Email already exists!!!",
                    IsSuccessful = false
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new BaseResponse
                {
                    Message = "Password does not match",
                    IsSuccessful = false
                };
            }

            var role = await _roleRepository.GetAsync(r => r.Name == "Customer");
            if (role == null)
            {
                return new BaseResponse
                {
                    Message = "Customer Role does not exists",
                    IsSuccessful = false
                };
            }

            var user = new User
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                RoleId = role.Id,
                Role = role,
                CreatedBy = "1",
                Gender = request.Gender,
                ImageUrl = await _fileRepository.UploadAsync(request.Image),
                PhoneNumber = request.PhoneNumber
            };

            role.Users.Add(user);
            _roleRepository.Update(role);
            await _userRepository.AddAsync(user);

            var wallet = new Wallet
            {
                CreatedBy = "0",
                UserId = user.Id,
                User = user
            };

            var customer = new Customer
            {
                UserId = user.Id,
                User = user,
                CreatedBy = "1",
                Wallet = wallet,
                WalletId = wallet.Id
            };


            await _walletRepository.AddAsync(wallet);
            await _customerRepository.AddAsync(customer);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Registration Successfull!!!",
                IsSuccessful = true,
            };
        }

        public async Task<BaseResponse<ICollection<CustomersResponse>>> GetAllCustomer()
        {
            var customers = await _customerRepository.GetAllAsync();
            return new BaseResponse<ICollection<CustomersResponse>>
            {
                Message = "List of Customers",
                IsSuccessful = true,
                Value = customers.Select(x => new CustomersResponse
                {
                    FullName = x.User.FirstName + " " + x.User.LastName,
                    Age = DateTime.Now.Year - x.User.DateOfBirth.Year,
                    Gender = x.User.Gender,
                    Id = x.Id,
                    NoOfOrdersMade = x.Orders.Count
                }).ToList()
            };
        }

        public async Task<BaseResponse<CustomerResponse>> GetCustomer(int id)
        {
            var customer = await _customerRepository.GetAsync(id);
            if (customer == null)
            {
                return new BaseResponse<CustomerResponse>
                {
                    Message = "Customer does not exists",
                    IsSuccessful = false
                };
            }
            return new BaseResponse<CustomerResponse>
            {
                Message = "Customer found successfully",
                IsSuccessful = true,
                Value = new CustomerResponse
                {
                    Id = customer.Id,
                    Age = DateTime.Now.Year - customer.User.DateOfBirth.Year,
                    Gender = customer.User.Gender,
                    FullName = customer.User.FirstName + " " + customer.User.LastName,
                    PhoneNumber = customer.User.PhoneNumber,
                    ImageUrl = customer.User.ImageUrl,
                }
            };
        }

        public async Task<BaseResponse<CustomerResponse>> GetCustomerByUserId(int id)
        {
            var customers = await _customerRepository.GetAllAsync();
            var customer = customers.FirstOrDefault(x => x.UserId == id);
            if (customer == null)
            {
                return new BaseResponse<CustomerResponse>
                {
                    Message = "Customer does not exists",
                    IsSuccessful = false
                };
            }
            return new BaseResponse<CustomerResponse>
            {
                Message = "Customer found successfully",
                IsSuccessful = true,
                Value = new CustomerResponse
                {
                    Id = customer.Id,
                    FullName = customer.User.FirstName + " " + customer.User.LastName,
                    Age = DateTime.Now.Year - customer.User.DateOfBirth.Year,
                    Gender = customer.User.Gender,
                    PhoneNumber = customer.User.PhoneNumber,
                    ImageUrl = customer.User.ImageUrl
                }
            };
        }

        public async Task<BaseResponse> RemoveCustomer(int id)
        {
            var customer = await _customerRepository.GetAsync(id);
            if (customer == null)
            {
                return new BaseResponse
                {
                    Message = "Customer does not exists",
                    IsSuccessful = false
                };
            }

            _customerRepository.Remove(customer);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Customer deleted successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> UpdateCustomer(int id, UpdateCustomerRequest request)
        {
            var customer = await _customerRepository.GetAsync(id);
            if (customer == null)
            {
                return new BaseResponse
                {
                    Message = "Customer does not exist",
                    IsSuccessful = false
                };
            }

            var exists = await _customerRepository.ExistsAsync(request.Email, id);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = "Email already exists!!!",
                    IsSuccessful = false
                };
            }

            var loginUser = _httpContext.HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;

            customer.User.FirstName = request.FirstName ?? customer.User.FirstName;
            customer.User.LastName = request.LastName ?? customer.User.LastName;
            customer.User.Email = request.Email ?? customer.User.Email;
            customer.User.DateOfBirth = request.DateOfBirth ?? customer.User.DateOfBirth;
            customer.User.DateModified = DateTime.Now;
            customer.User.ModifiedBy = loginUser;
            customer.DateModified = DateTime.Now;
            customer.ModifiedBy = loginUser;
            customer.User.PhoneNumber = request.PhoneNumber ?? customer.User.PhoneNumber;
            customer.User.Gender = request.Gender ?? customer.User.Gender;
            customer.User.ImageUrl = await _fileRepository.UploadAsync(request.Image) ?? customer.User.ImageUrl;

            _customerRepository.Update(customer);

            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Customer updated successfully",
                IsSuccessful = true
            };
        }

        public async Task<int> GetLoginCustomerId()
        {
            var loginUserEmail = _httpContext.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Email).Value;
            var customer = await _customerRepository.GetAsync(x => x.User.Email == loginUserEmail);
            return customer.Id;
        }
    }
}
