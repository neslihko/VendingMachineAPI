using VendingMachineAPI.Model;

namespace VendingMachineAPI.Services
{
    public interface IProductService
    {
        Product? AddProduct(Product newProduct, int sellerId, int userId);

        Product? GetProductById(int productId);

        void UpdateProduct(Product product);
    }
}
