using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VendingMachineAPI.Services;
using VendingMachineAPI.Dtos;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using VendingMachineAPI.Model;

namespace VendingMachineAPI.Controllers.Tests
{
    [TestClass()]
    public class ProductControllerTests
    {
        [TestMethod()]
        public void GetProductById_ReturnsOkResult()
        {
            // Arrange
            int productId = 1;
            var productServiceMock = new Mock<IProductService>();
            var mapperMock = new Mock<IMapper>();
            var controller = new ProductController(productServiceMock.Object, mapperMock.Object);

            // Act
            var result = controller.GetProductById(productId);

            // Assert
            Assert.IsInstanceOfType<OkObjectResult>(result);
        }


        [TestMethod()]
        public void Update_ValidInput_ReturnsOkResult()
        {
            // Arrange
            int productId = 1;
            var productDto = new ProductDto { /* initialize with valid data */ };
            var productServiceMock = new Mock<IProductService>();
            var mapperMock = new Mock<IMapper>();
            var controller = new ProductController(productServiceMock.Object, mapperMock.Object);

            // Act
            var result = controller.Update(productId, productDto);

            // Assert
            Assert.IsInstanceOfType<OkResult>(result);
        }

        [TestMethod()]
        public void AddProduct_ValidInput_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var newProduct = new Product { };
            int sellerId = 123;
            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.AddProduct(It.IsAny<Product>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new Product { ProductId = 1 });
            var mapperMock = new Mock<IMapper>();
            var controller = new ProductController(productServiceMock.Object, mapperMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, "buyer123")
                    }))
                }
            };

            // Act
            var result = controller.AddProduct(newProduct, sellerId);

            // Assert
            Assert.IsInstanceOfType<CreatedAtActionResult>(result);
            Assert.AreEqual(1, (result as CreatedAtActionResult)?.RouteValues?["id"]);
        }
    }
}