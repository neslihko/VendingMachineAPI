using VendingMachineAPI.Helpers;
using VendingMachineAPI.Model;

namespace VendingMachineAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;
        private UserService _userService;

        public ProductService(DataContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public Product? GetProductById(int productId) =>
            _context.Products.FirstOrDefault(p => p.ProductId == productId);

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public Product? AddProduct(Product newProduct, int sellerId, int userId)
        {
            if (!IsUserSeller(userId))
            {
                // Throw an exception or return an appropriate response
                throw new UnauthorizedAccessException("Only users with the 'seller' role can add products");
            }

            // You might want to check if the sellerId is valid or if the user has the "seller" role
            // (Authentication and authorization checks can be more elaborate in a real-world scenario)


            // Set the sellerId for the new product
            newProduct.SellerId = sellerId;

            // Validate product cost (should be in multiples of 5)
            if (newProduct.Cost % 5 != 0)
            {
                throw new AppException("Product cost should be in multiples of 5");
            }

            // You might want to perform additional validation or business logic
            _context.Products.Add(newProduct);
            _context.SaveChanges();

            // Return the created product
            return newProduct;
        }

        private bool IsUserSeller(int userId)
        {
            var user = _userService.GetById(userId);

            return user != null && user.Role == "seller";
        }
    }
}
