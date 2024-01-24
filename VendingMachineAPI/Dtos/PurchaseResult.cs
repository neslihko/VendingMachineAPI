using VendingMachineAPI.Model;

namespace VendingMachineAPI.Dtos
{
    public class PurchaseResult
    {
        public decimal TotalSpent { get; set; }
        public Product PurchasedProduct { get; set; }
        public Dictionary<int, int> Change { get; set; } // Dictionary representing the change in coins
    }
}
