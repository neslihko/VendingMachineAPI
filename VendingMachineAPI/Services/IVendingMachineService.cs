using VendingMachineAPI.Dtos;

namespace VendingMachineAPI.Services
{
    public interface IVendingMachineService
    {
        void DepositCoins(int buyerUserId, int coinValue);
        PurchaseResult BuyProduct(int buyerUserId, int productId, int amount);
        void ResetDeposit(int buyerUserId);
    }
}
