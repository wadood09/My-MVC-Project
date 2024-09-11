using MyMVCProject.Core.Application.Interface.Repositories;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Models;
using MyMVCProject.Models.WalletModel;
using System.Security.Claims;

namespace MyMVCProject.Core.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletRepository _walletRepository;
        private readonly IHttpContextAccessor _httpContext;

        public WalletService(IUnitOfWork unitOfWork, IWalletRepository walletRepository, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _walletRepository = walletRepository;
            _httpContext = httpContext;
        }

        public async Task<BaseResponse<WalletResponse>> GetLoginUserWallet()
        {
            var loginUserId = _httpContext.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var wallet = await _walletRepository.GetAsync(x => x.UserId == int.Parse(loginUserId));
            if(wallet == null )
            {
                return new BaseResponse<WalletResponse>
                {
                    Message = "User does not have a wallet account",
                    IsSuccessful = false
                };
            }

            return new BaseResponse<WalletResponse>
            {
                Message = "Wallet successfully found",
                IsSuccessful = true,
                Value = new WalletResponse
                {
                    Balance = wallet.Balance,
                    Id = wallet.Id,
                    UserId = wallet.UserId
                }
            };
        }

        public async Task<BaseResponse<WalletResponse>> GetWallet(int id)
        {
            var wallet = await _walletRepository.GetAsync(id);
            if(wallet == null )
            {
                return new BaseResponse<WalletResponse>
                {
                    Message = "Wallet does not exists",
                    IsSuccessful = false
                };
            }

            return new BaseResponse<WalletResponse>
            {
                Message = "Wallet successfully found",
                IsSuccessful = true,
                Value = new WalletResponse
                {
                    Balance = wallet.Balance,
                    Id = wallet.Id,
                    UserId = wallet.UserId
                }
            };
        }

        public async Task<BaseResponse> UpdateWallet(int id, UpdateWalletRequest request)
        {
            var wallet = await _walletRepository.GetAsync(id);
            if (wallet == null)
            {
                return new BaseResponse
                {
                    Message = "User does not have a wallet account",
                    IsSuccessful = false
                };
            }

            if (request.Balance < 100)
            {
                return new BaseResponse
                {
                    Message = $"The lowest amount that can be deposited at once is #100",
                    IsSuccessful = false
                };
            }

            if (request.Balance > 1000000)
            {
                return new BaseResponse
                {
                    Message = $"The highest amount that can be deposited at once is #1,000,000",
                    IsSuccessful = false
                };
            }

            wallet.Balance += request.Balance;
            _walletRepository.Update(wallet);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "Walet balance successfully increased",
                IsSuccessful = true
            };
        }
    }
}
