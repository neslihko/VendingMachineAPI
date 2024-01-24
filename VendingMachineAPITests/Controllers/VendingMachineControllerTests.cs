using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Security.Claims;
using VendingMachineAPI.Dtos;
using VendingMachineAPI.Model;
using VendingMachineAPI.Services;

namespace VendingMachineAPI.Controllers.Tests
{
    [TestClass]
    public class VendingMachineControllerTests
    {
        private Mock<IUserService> userService;
        private Mock<IProductService> productService;
        private IVendingMachineService vendingMachineService;
        private VendingMachineController controller;
        private int OkUserID = 99;

        [TestInitialize]
        public void InitTest()
        {
            userService = new Mock<IUserService>();
            productService = new Mock<IProductService>();
            vendingMachineService = new VendingMachineService(userService.Object, productService.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, OkUserID.ToString()),
                    new Claim(ClaimTypes.Name, $"User no {OkUserID}"),
                }, "mock"));

            controller = new VendingMachineController(vendingMachineService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                }
            };
        }

        [TestMethod]
        [DataRow(10, 5, 49, false)]
        [DataRow(10, 3, 51, true)]
        public void BuyProduct_WithEnoughAmounts_ReturnsExpectedResults(int productCost, int amount, int deposit, bool supposedToSucceed)
        {
            int productId = 55;
            userService.Setup(u => u.GetById(OkUserID)).Returns(new Model.User()
            {
                Role = "Seller",
                UserId = OkUserID
            });

            productService.Setup(p => p.GetProductById(productId)).Returns(new Product()
            {
                ProductId = productId,
                Cost = productCost,
                AmountAvailable = amount + 10
            });

            var currentUser = new User()
            {
                UserId = OkUserID,
                Deposit = deposit,
                Username = $"U{OkUserID}"
            };

            userService.Setup(u => u.GetById(OkUserID)).Returns(currentUser);

            var request = new BuyRequest()
            {
                Amount = amount,
                ProductId = productId
            };

            IActionResult result = default;

            try
            {
                result = controller.BuyProduct(request);
            }
            catch (Exception)
            {
            }

            var returnValue = result as ObjectResult;
            var calledSuccessfully = (returnValue?.StatusCode == (int)HttpStatusCode.OK);

            Assert.AreEqual(supposedToSucceed, calledSuccessfully);

            if (supposedToSucceed)
            {
                Assert.IsNotNull(result);

                var purchaseResult = returnValue?.Value as PurchaseResult;

                Assert.IsNotNull(purchaseResult);
                Assert.AreEqual(productCost * amount, purchaseResult.TotalSpent);
            }
        }

        [TestMethod]

        [DataRow(5, true)]
        [DataRow(10, true)]
        [DataRow(20, true)]
        [DataRow(50, true)]
        [DataRow(100, true)]
        [DataRow(0, false)]
        [DataRow(1, false)]
        [DataRow(11, false)]
        [DataRow(21, false)]
        [DataRow(51, false)]
        [DataRow(101, false)]
        public void DepositProduct_ReturnsExpectedResults(int coinValue, bool supposedToSucceed)
        {
            userService.Setup(u => u.GetById(OkUserID)).Returns(new User()
            {
                Role = "Seller",
                UserId = OkUserID
            });


            var currentUser = new User()
            {
                UserId = OkUserID,
                Role = "Role"
            };

            userService.Setup(u => u.GetById(OkUserID)).Returns(currentUser);

            IActionResult result = default;

            try
            {
                result = controller.DepositCoins(coinValue);
            }
            catch (Exception)
            {
            }

            var returnValue = result as ObjectResult;
            var calledSuccessfully = (returnValue?.StatusCode == (int)HttpStatusCode.OK);

            Assert.AreEqual(supposedToSucceed, calledSuccessfully);
        }
    }
}