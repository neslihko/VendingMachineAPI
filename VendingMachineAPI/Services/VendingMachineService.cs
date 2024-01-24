using VendingMachineAPI.Dtos;
using VendingMachineAPI.Helpers;
using VendingMachineAPI.Model;

namespace VendingMachineAPI.Services
{
    public class VendingMachineService : IVendingMachineService
    {
        private readonly IUserService _users;

        private readonly IProductService _products;

        public VendingMachineService(IUserService userService, IProductService productService)
        {
            _products = productService;
            _users = userService;
        }

        public void DepositCoins(int buyerUserId, int coinValue)
        {
            // Validate the user exists and has the "buyer" role
            var buyerUser = _users.GetById(buyerUserId);
            if (buyerUser == null)
            {
                throw new AppException("User not authorized to deposit coins.");
            }

            // Validate the coin value
            ValidateCoinValue(coinValue);

            // Add the coin value to the user's deposit
            _users.DepositCoin(buyerUser.UserId, coinValue);
        }

        public PurchaseResult BuyProduct(int buyerUserId, int productId, int amount)
        {
            // Validate and get user
            var buyerUser = ValidateAndGetBuyerUser(buyerUserId);

            // Validate and get product
            var productToBuy = ValidateAndGetProduct(productId);

            // Validate user has enough deposit
            ValidateSufficientFunds(buyerUser, productToBuy.Cost, amount);

            // Perform the purchase
            PerformPurchase(buyerUser, productToBuy, amount);

            // Return the purchase result
            return new PurchaseResult
            {
                TotalSpent = productToBuy.Cost * amount,
                PurchasedProduct = productToBuy,
                Change = CalculateChange(productToBuy.Cost * amount)
            };
        }

        private User ValidateAndGetBuyerUser(int buyerUserId) =>
            _users.GetById(buyerUserId) ??
            throw new AppException("User not authorized to buy products.");

        private Product ValidateAndGetProduct(int productId) =>
            _products.GetProductById(productId) ??
            throw new AppException("Product not found.");

        private void ValidateSufficientFunds(User buyerUser, decimal costPerUnit, int amount)
        {
            var totalCost = costPerUnit * amount;

            if (buyerUser.Deposit < totalCost)
            {
                throw new AppException("Insufficient funds to buy the product.");
            }
        }

        private void PerformPurchase(User buyerUser, Product productToBuy, int amount)
        {
            // Update other data (e.g., decrease product quantity)
            // Deduct the deposit
            buyerUser.Deposit -= productToBuy.Cost * amount;

            // Update user and product in the database (assuming _users and _products interact with the database)
            _users.Update(buyerUser);
            _products.UpdateProduct(productToBuy);
        }

        private void ValidateCoinValue(int coinValue)
        {
            // Validate that the coin value is one of the accepted values 
            if (!Constants.AllowedDenominations.Contains(coinValue))
            {
                throw new ArgumentException($"Invalid coin value. Accepted values: {string.Join(", ", Constants.AllowedDenominations)}");
            }
        }

        private static Dictionary<int, int> CalculateChange(decimal changeAmount)
        {
            // Logic to calculate the change in coins
            // For simplicity, let's assume an infinite supply of each coin value
            var change = new Dictionary<int, int>();

            var coinValues = Constants
                .AllowedDenominations
                .OrderByDescending(x => x)
                .ToArray();

            foreach (var coinValue in coinValues)
            {
                if (changeAmount >= coinValue)
                {
                    decimal numberOfCoins = changeAmount / coinValue;
                    change.Add(coinValue, Convert.ToInt32(numberOfCoins));
                    changeAmount -= numberOfCoins * coinValue;
                }
            }

            return change;
        }

        public void ResetDeposit(int buyerUserId)
        {
            var buyerUser = _users.GetById(buyerUserId);
            if (buyerUser == null)
            {
                throw new AppException("User not authorized to reset deposit.");
            }

            buyerUser.Deposit = 0;
            _users.Update(buyerUser);
        }
    }
}
