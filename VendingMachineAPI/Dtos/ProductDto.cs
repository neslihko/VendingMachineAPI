namespace VendingMachineAPI.Dtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public int SellerId { get; set; }
        public int AmountAvailable { get; set; }
        public decimal Cost { get; set; }
        public string ProductName { get; set; }
    }
}
