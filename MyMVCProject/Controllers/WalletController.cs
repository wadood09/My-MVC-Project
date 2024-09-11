using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMVCProject.Core.Application.Interface.Services;
using MyMVCProject.Core.Application.Services;
using MyMVCProject.Models.ViewModels;
using MyMVCProject.Models.WalletModel;

namespace MyMVCProject.Controllers
{
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly NavigationHistoryManager _historyManager;
        public WalletController(IWalletService walletService, NavigationHistoryManager historyManager)
        {
            _walletService = walletService;
            _historyManager = historyManager;
        }

        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> GetWallet()
        {
            _historyManager.AddToHistory("Wallet", nameof(GetWallet));

            var wallet = await _walletService.GetLoginUserWallet();
            if(wallet.IsSuccessful)
            {
                return View(wallet.Value);
            }
            TempData["ErrorMessage"] = wallet.Message;
            return RedirectToAction("GoBack", "User");
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateWallet(int id)
        {
            var param = new Dictionary<string, string>
            {
                {"id", id.ToString() }
            };
            _historyManager.AddToHistory("Wallet", nameof(UpdateWallet), param);

            var wallet = await _walletService.GetWallet(id);
            if(wallet.IsSuccessful)
            {
                var model = new UpdateWalletViewModel
                {
                    Balance = wallet.Value.Balance,
                    Id = wallet.Value.Id
                };
                return View(model);
            }
            return RedirectToAction("GoBack", "User");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWallet(int id, UpdateWalletRequest request)
        {
            var wallet = await _walletService.UpdateWallet(id, request);
            if(wallet.IsSuccessful)
            {
                return RedirectToAction("GetWallet");
            }
            TempData["ErrorMessage"] = wallet.Message;
            var model = new UpdateWalletViewModel
            {
                Balance = request.Balance,
                Id = id
            };
            return View(model);
        }
    }
}
