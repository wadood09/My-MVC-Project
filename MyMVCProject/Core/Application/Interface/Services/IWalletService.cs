using MyMVCProject.Models;
using MyMVCProject.Models.WalletModel;

namespace MyMVCProject.Core.Application.Interface.Services
{
    public interface IWalletService
    {
        public Task<BaseResponse<WalletResponse>> GetLoginUserWallet();
        public Task<BaseResponse> UpdateWallet(int id, UpdateWalletRequest request);
        Task<BaseResponse<WalletResponse>> GetWallet(int id);
    }
}
