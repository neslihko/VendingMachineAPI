namespace VendingMachineAPI.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using VendingMachineAPI.Dtos;
    using VendingMachineAPI.Helpers;
    using VendingMachineAPI.Model;
    using VendingMachineAPI.Services;

    [Authorize(Roles = "seller")]
    [Route("[controller]")]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(
            IProductService productService,
            IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _productService.GetProductById(id);
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            try
            {
                _productService.UpdateProduct(product);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("product")]
        public IActionResult AddProduct([FromBody] Product newProduct, int sellerId)
        {
            try
            {
                // Call ProductService to add the product
                var createdProduct = _productService.AddProduct(newProduct, sellerId, CurrentBuyerId);

                // Return the created product in the response
                return CreatedAtAction(nameof(createdProduct), new { id = createdProduct.ProductId }, createdProduct);
            }
            catch (ArgumentException ex)
            {
                // Handle validation or business logic errors
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access errors
                return Forbid();
            }
            catch (Exception)
            {
                // Handle other unexpected errors
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
