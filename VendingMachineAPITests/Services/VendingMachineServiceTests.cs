using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VendingMachineAPI.Dtos;
using VendingMachineAPI.Helpers;
using VendingMachineAPI.Model;

namespace VendingMachineAPI.Services.Tests
{
    [TestClass()]
    public class VendingMachineServiceTests
    {
        private IVendingMachineService vendingMachineService;
        private Mock<IUserService> userService;
        private Mock<IProductService> productService;
        private Mock<User> user;
        private const int OkUserID = 99;
        private const int OkProductID = 101;

        [TestInitialize]
        public void InitTest()
        {
            //Arrange 
            user = new Mock<User>();
            userService = new Mock<IUserService>();
            productService = new Mock<IProductService>();
            vendingMachineService = new VendingMachineService(userService.Object, productService.Object);
        }

        [TestMethod()]
        [DataRow(5)]
        [DataRow(10)]
        [DataRow(20)]
        [DataRow(50)]
        [DataRow(100)]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(11)]
        [DataRow(21)]
        [DataRow(51)]
        [DataRow(101)]
        public void InvalidUser_AnyAmount_CantDepositCoins(int depositAmount)
        {
            try
            {
                // This line should throw ex, so Assert.Fail should not execute.
                vendingMachineService.DepositCoins(OkUserID, depositAmount);

                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType<AppException>(ex);
                Assert.AreEqual("User not authorized to deposit coins.", ex.Message);
            }
        }

        [TestMethod()]
        [DataRow(5)]
        [DataRow(10)]
        [DataRow(20)]
        [DataRow(50)]
        [DataRow(100)]
        public void ValidUser_ValidAmount_CanDepositCoins(int depositAmount)
        {
            userService.Setup(u => u.GetById(OkUserID)).Returns(user.Object);
            try
            {
                vendingMachineService.DepositCoins(OkUserID, depositAmount);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(11)]
        [DataRow(21)]
        [DataRow(51)]
        [DataRow(101)]
        public void ValidUser_InvalidAmount_CantDepositCoins(int depositAmount)
        {
            userService.Setup(u => u.GetById(OkUserID)).Returns(user.Object);

            try
            {
                // This line should throw ex, so Assert.Fail should not execute.
                vendingMachineService.DepositCoins(OkUserID, depositAmount);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType<ArgumentException>(ex);
            }
        }

        [TestMethod()]
        [DataRow(10.0, 2, 20.0, true)]
        [DataRow(10.0, 2, 19.0, false)]
        [DataRow(250.0, 10, 2500.0, true)]
        [DataRow(250.0, 10, 2499.0, false)]
        public void BuyProduct_RespectsUserDepositAndTotalCost(double productCost, int amount, double userDeposit, bool canBuy)
        {
            var product = new Product()
            {
                ProductId = OkProductID,
                AmountAvailable = amount + 10,
                Cost = (decimal)productCost,
                ProductName = $"P{OkProductID}"
            };

            var currentUser = new User()
            {
                UserId = OkUserID,
                Deposit = (decimal)userDeposit,
                Username = $"U{OkUserID}"
            };

            productService.Setup(p => p.GetProductById(OkProductID)).Returns(product);
            userService.Setup(u => u.GetById(OkUserID)).Returns(currentUser);

            PurchaseResult result = default;

            try
            {
                result = vendingMachineService.BuyProduct(OkUserID, OkProductID, amount);
            }
            catch
            {
            }

            var bought = result != null;
            Assert.AreEqual(bought, canBuy);

            if (canBuy)
            {
                Assert.IsNotNull(result);
                Assert.AreEqual((decimal)(amount * productCost), result.TotalSpent);
            }
        }
    }
}